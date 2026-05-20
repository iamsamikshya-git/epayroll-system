using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using E_PayRoll.Data;
using E_PayRoll.Models;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using E_PayRoll.Services;
using E_PayRoll.Models; 


// iTextSharp
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace E_PayRoll.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notifications;

        public SalaryController(ApplicationDbContext context, INotificationService notifications)
        {
            _context = context;
            _notifications = notifications;
        }

        // -------------------- Helpers --------------------

       private int GetCurrentSchoolId()
{
    if (!User.Identity.IsAuthenticated)
        throw new InvalidOperationException("User is not authenticated.");

    // Get logged-in user's ID (assuming it's stored in ClaimTypes.NameIdentifier)
    var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (userIdClaim == null)
        throw new InvalidOperationException("User ID claim not found.");

    if (!int.TryParse(userIdClaim, out int userId))
        throw new InvalidOperationException("User ID claim is invalid.");

    // Look up the School record where UserId matches logged-in user ID
    var school = _context.Schools.FirstOrDefault(s => s.UserId == userId);
    if (school == null)
        throw new InvalidOperationException("School not found for current user.");

    return school.Id;
}

        private async Task LoadTeachersForCurrentSchoolAsync()
        {
            var schoolId = GetCurrentSchoolId();

            var teacherItems = await _context.Teachers
                .Where(t => t.SchoolId == schoolId)
                .Select(t => new
                {
                    t.Id,
                    FullName = t.FirstName +
                               (string.IsNullOrEmpty(t.MiddleName) ? " " : " " + t.MiddleName + " ") +
                               t.LastName
                })
                .OrderBy(t => t.FullName)
                .ToListAsync();

            ViewBag.Teachers = teacherItems
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.FullName })
                .ToList();
        }

        private async Task<string> GetTeacherFullNameAsync(int teacherId, int schoolId)
        {
            var name = await _context.Teachers
                .Where(t => t.Id == teacherId && t.SchoolId == schoolId )
                .Select(t => t.FirstName +
                             (string.IsNullOrEmpty(t.MiddleName) ? " " : " " + t.MiddleName + " ") +
                             t.LastName)
                .FirstOrDefaultAsync();

            return name; // null if invalid selection / not in this school
        }

        // -------------------- Create (SalaryCalculation) --------------------

        [HttpGet]
        public async Task<IActionResult> SalaryCalculation()
        {
            await LoadTeachersForCurrentSchoolAsync();
            var grades = await _context.GradeNumbers
    .OrderBy(g => g.Number)   // Sort by grade number ascending: 1, 2, 3, ...
    .Select(g => new SelectListItem
    {
        Value = g.Number.ToString(),
        Text = g.Number.ToString()  // Or you can use any descriptive text if available
    })
    .ToListAsync();

ViewBag.Grades = grades;
            return View("~/Views/School/Salary/TeacherSalaryCalculation.cshtml", new Salary());
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SalaryCalculation(Salary model)
{
    // We set this on the server; don't validate an empty posted value
    ModelState.Remove(nameof(Salary.TeacherName));

    var schoolId = GetCurrentSchoolId();

    // Validate TeacherId and resolve full name from the DB
    if (string.IsNullOrWhiteSpace(model.TeacherId) || !int.TryParse(model.TeacherId, out var teacherId))
    {
        ModelState.AddModelError(nameof(Salary.TeacherId), "Select a valid teacher.");
    }
    else
    {
        var fullName = await GetTeacherFullNameAsync(teacherId, schoolId);
        if (string.IsNullOrEmpty(fullName))
            ModelState.AddModelError(nameof(Salary.TeacherId), "Selected teacher is not in your school.");
        else
            model.TeacherName = fullName; // snapshot
    }

    if (!ModelState.IsValid)
    {
        await LoadTeachersForCurrentSchoolAsync();
        ViewBag.Grades = await _context.GradeNumbers
            .OrderBy(g => g.Number)
            .Select(g => new SelectListItem { Value = g.Number.ToString(), Text = g.Number.ToString() })
            .ToListAsync();

        return View("~/Views/School/Salary/TeacherSalaryCalculation.cshtml", model);
    }
      model.Status = "Pending";
    _context.Salaries.Add(model);
    await _context.SaveChangesAsync();
    // 🔔 Notify the Admin who manages this school
//var schoolId = GetCurrentSchoolId();

// Get school name (for message) + admin's username (recipient)
var schoolInfo = await (from sc in _context.Schools
                        where sc.Id == schoolId
                        select new
                        {
                            sc.SchoolName,
                            AdminUsername =
                                (from a in _context.Admins
                                 join u in _context.Users on a.UserId equals u.Id
                                 where a.Id == sc.AdminId
                                 select u.Username).FirstOrDefault()
                        }).FirstOrDefaultAsync();

if (schoolInfo?.AdminUsername != null)
{
    await _notifications.CreateAsync(
        recipientUsername: schoolInfo.AdminUsername,
        title: "Salary submitted",
        message: $"{schoolInfo.SchoolName} submitted a salary request (ID #{model.Id}).",
        linkUrl: Url.Action("AckSalaryList", "Salary", null, Request.Scheme),
        type: NotificationType.SalarySubmitted
    );
}

    TempData["Success"] = "Salary request submitted.";
    return RedirectToAction(nameof(SalaryList));
}

        [HttpGet]
        public async Task<IActionResult> GetTeacherDefaults(int teacherId)
        {
            var schoolId = GetCurrentSchoolId();

            // 1) Teacher from this school only
            var teacher = await _context.Teachers
                .AsNoTracking()
                .Where(t => t.Id == teacherId && t.SchoolId == schoolId)
                .Select(t => new
                {
                    t.AppointmentType,   // string
                    LevelName = t.TeacherLevel, // string in Teacher table
                    CategoryName = t.TeacherCategory // string in Teacher table
                })
                .FirstOrDefaultAsync();

            if (teacher == null) return NotFound("Teacher not found in your school.");

            // 2) Map Level/Category *names* → IDs (case-insensitive, trimmed)
            var levelName = (teacher.LevelName ?? string.Empty).Trim();
            var categoryName = (teacher.CategoryName ?? string.Empty).Trim();

            // NOTE: adjust table/entity names/properties if yours differ
            var levelId = await _context.TeacherLevels
                .Where(l => l.Name.ToLower() == levelName.ToLower())
                .Select(l => (int?)l.Id)
                .FirstOrDefaultAsync();

            var categoryId = await _context.TeacherCategories
                .Where(c => c.Name.ToLower() == categoryName.ToLower())
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync();

            decimal? basicSalary = null;

            // 3) If both IDs resolved, pull BasicSalaries row by FK pair
            if (levelId.HasValue && categoryId.HasValue)
            {
                basicSalary = await _context.BasicSalaries
                    .Where(b => b.TeacherLevelId == levelId.Value
                                && b.TeacherCategoryId == categoryId.Value)
                    .Select(b => (decimal?)b.BaseSalaryAmount) // adjust property name if needed
                    .FirstOrDefaultAsync();
            }

            return Json(new
            {
                appointmentType = teacher.AppointmentType,
                level = teacher.LevelName,
                category = teacher.CategoryName,
                levelId = levelId,          // useful if you also need to set hidden fields
                categoryId = categoryId,    // "
                basicSalary = basicSalary    // null if not found
            });
        }


        // -------------------- Municipality (Admin) helpers --------------------

private async Task LoadSchoolsForAdminAsync()
{
    // If you have municipality scoping, filter here; else show all schools
    ViewBag.Schools = await _context.Schools
        .OrderBy(s => s.SchoolName)
        .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.SchoolName })
        .ToListAsync();
}

        // AJAX for Teacher dropdown on the Add page
        [HttpGet]
        public async Task<IActionResult> GetTeachersBySchool(int schoolId)
        {
            var teachers = await _context.Teachers
                .Where(t => t.SchoolId == schoolId)
                .OrderBy(t => t.FirstName).ThenBy(t => t.LastName)
                .Select(t => new
                {
                    id = t.Id,
                    name = t.FirstName +
                           (string.IsNullOrEmpty(t.MiddleName) ? " " : " " + t.MiddleName + " ") +
                           t.LastName
                })
                .ToListAsync();

            return Json(teachers);
        }
        // SCHOOL view (school’s own salary list)
[Authorize(Roles = "School")]
[HttpGet]
public async Task<IActionResult> SalaryList()
{
    var schoolId = GetCurrentSchoolId();

    var vm = await _context.Salaries.AsNoTracking()
        .Where(s => _context.Teachers
            .Any(t => t.Id.ToString() == s.TeacherId && t.SchoolId == schoolId))
        .OrderByDescending(s => s.Id)
        .Select(s => new SalaryViewModel {
            Id = s.Id,
            FullName = s.TeacherName,
            BasicSalary = s.BasicSalary,
            PF = s.EmployeesProvidentFund,
            TotalSalary = s.TotalSalary,
            Status = s.Status
        })
        .ToListAsync();

    return View("~/Views/School/Salary/SalaryCalculationList.cshtml", vm);
}

        // -------------------- Municipality (Admin) Add page --------------------

        [HttpGet]
        // [Authorize(Roles = "Admin,SuperAdmin")] // uncomment if you have role-based auth
// Get MunicipalityId of the logged-in Admin/Municipality user.
private int GetCurrentMunicipalityId()
{
    if (!User.Identity.IsAuthenticated)
        throw new InvalidOperationException("Not authenticated.");

    // Prefer a claim if you stamp it at sign-in
    var claim = User.FindFirst("MunicipalityId")?.Value;
    if (int.TryParse(claim, out var muniId) && muniId > 0) return muniId;

    // Fallback: resolve from your Municipalities/Admins table by UserId
    var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new InvalidOperationException("UserId claim missing.");
    if (!int.TryParse(userIdStr, out var userId))
        throw new InvalidOperationException("Invalid UserId claim.");

    // Adjust table/entity names to your schema
    var found = _context.Admins
        .Where(m => m.UserId == userId)
        .Select(m => (int?)m.Id)
        .FirstOrDefault();

    if (!found.HasValue)
        throw new InvalidOperationException("Municipality not found for current user.");

    return found.Value;
}

// Populate School dropdown for THIS municipality only
private async Task LoadSchoolsForCurrentMunicipalityAsync()
{
    var muniId = GetCurrentMunicipalityId();

    ViewBag.Schools = await _context.Schools
        .Where(sc => sc.AdminId == muniId)
        .OrderBy(sc => sc.SchoolName)
        .Select(sc => new SelectListItem { Value = sc.Id.ToString(), Text = sc.SchoolName })
        .ToListAsync();
}
        [HttpGet]
//[Authorize(Roles = "Admin,SuperAdmin")]
public async Task<IActionResult> Add(int? schoolId, int? teacherId, DateTime? startDate, DateTime? endDate)
{
    await LoadSchoolsForCurrentMunicipalityAsync();
    var muniId = GetCurrentMunicipalityId();

    if (endDate.HasValue) endDate = endDate.Value.Date.AddDays(1).AddTicks(-1); // inclusive

    // Start with municipality-scoped join: Salary -> Teacher -> School
    var q =
        from s in _context.Salaries.AsNoTracking()
        join t in _context.Teachers on s.TeacherId equals t.Id.ToString()
        join sc in _context.Schools on t.SchoolId equals sc.Id
        where sc.AdminId == muniId
        select new { s, t, sc };

    if (schoolId.HasValue && schoolId > 0)
        q = q.Where(x => x.sc.Id == schoolId.Value);

    if (teacherId.HasValue && teacherId > 0)
        q = q.Where(x => x.t.Id == teacherId.Value);

    if (startDate.HasValue)
        q = q.Where(x => x.s.Date >= startDate.Value);

    if (endDate.HasValue)
        q = q.Where(x => x.s.Date <= endDate.Value);

    var vm = await q
        .OrderByDescending(x => x.s.Id)
        .Select(x => new SalaryAcknowledgmentViewModel
        {
            Id                  = x.s.Id,
            FullName            = x.s.TeacherName,
            IssuedDate          = x.s.Date,
            BasicSalary         = x.s.BasicSalary,
            PF                  = x.s.EmployeesProvidentFund,
            CIT                 = x.s.CitizenInvestmentTrust,
            Dearness            = x.s.DearnessAllowance,
            HeadmasterAllowance = x.s.HeadmasterAllowance,
            Clothing            = x.s.ClothingAllowance,
            Festival            = x.s.FestivalAllowance,
            TotalSalary         = x.s.TotalSalary,
            Status              = x.s.Status
        })
        .ToListAsync();

    ViewBag.SelectedSchoolId  = schoolId;
    ViewBag.SelectedTeacherId = teacherId;
    ViewBag.StartDate         = startDate?.ToString("yyyy-MM-dd");
    ViewBag.EndDate           = endDate?.ToString("yyyy-MM-dd");

    return View("~/Views/Admin/SalaryAcknowledgment/Add.cshtml", vm);
}

// -------------------- Municipality (Admin) actions --------------------


private async Task<string?> GetSchoolUsernameBySalaryJoinAsync(int salaryId)
{
    var username = await (
        from s  in _context.Salaries.AsNoTracking()
        where s.Id == salaryId
        join t  in _context.Teachers on s.TeacherId.Trim() equals t.Id.ToString()
        join sc in _context.Schools  on t.SchoolId equals sc.Id
        join u  in _context.Users    on sc.UserId  equals u.Id
        select u.Username
    ).FirstOrDefaultAsync();

    return string.IsNullOrWhiteSpace(username) ? null : username;
}
// [Authorize(Roles = "Admin,SuperAdmin")]
[HttpPost]

// [Authorize(Roles = "Admin,SuperAdmin")]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Approve(int id)
{
    var s = await _context.Salaries.FindAsync(id);
    if (s == null) return NotFound();

    if ((s.Status ?? "").Trim() != "Pending")
    {
        TempData["Error"] = "Only pending rows can be approved.";
        return Redirect(Request.Headers["Referer"].ToString());
    }

    s.Status = "Accepted";
    await _context.SaveChangesAsync();

    var schoolUsername = await GetSchoolUsernameBySalaryJoinAsync(id); // <- new helper
    if (!string.IsNullOrWhiteSpace(schoolUsername))
    {
        await _notifications.CreateAsync(
            recipientUsername: schoolUsername,
            title: "Salary approved",
            message: $"Your salary request (ID #{s.Id}) was approved.",
            linkUrl: Url.Action("SalaryList", "Salary", null, Request.Scheme),
            type: NotificationType.SalaryApproved
        );
    }

    TempData["Success"] = "Approved.";
    return Redirect(Request.Headers["Referer"].ToString());
}

[HttpPost]

// [Authorize(Roles = "Admin,SuperAdmin")]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Reject(int id)
{
    var s = await _context.Salaries.FindAsync(id);
    if (s == null) return NotFound();

    if ((s.Status ?? "").Trim() != "Pending")
    {
        TempData["Error"] = "Only pending rows can be rejected.";
        return Redirect(Request.Headers["Referer"].ToString());
    }

    s.Status = "Rejected";
    await _context.SaveChangesAsync();

    var schoolUsername = await GetSchoolUsernameBySalaryJoinAsync(id); // <- new helper
    if (!string.IsNullOrWhiteSpace(schoolUsername))
    {
        await _notifications.CreateAsync(
            recipientUsername: schoolUsername,
            title: "Salary rejected",
            message: $"Your salary request (ID #{s.Id}) was rejected.",
            linkUrl: Url.Action("SalaryList", "Salary", null, Request.Scheme),
            type: NotificationType.General
        );
    }

    TempData["Success"] = "Rejected.";
    return Redirect(Request.Headers["Referer"].ToString());
}
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ToggleDecision(int id)
{
    var s = await _context.Salaries.FindAsync(id);
    if (s == null) return NotFound();

    var current = (s.Status ?? "").Trim();

    if (current.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
    {
        s.Status = "Rejected";
    }
    else
    {
        // Pending -> Accepted, Rejected -> Accepted
        s.Status = "Accepted";
    }

    await _context.SaveChangesAsync();
    TempData["Success"] = $"Status set to {s.Status}.";
    return Redirect(Request.Headers["Referer"].ToString());
}

        [HttpGet]
[Authorize(Roles = "Admin,SuperAdmin")]
[Route("SalaryAcknowledgment/SalaryList")] // nice, unambiguous URL
public async Task<IActionResult> AckSalaryList(int? schoolId, int? teacherId, DateTime? startDate, DateTime? endDate)
{
    await LoadSchoolsForCurrentMunicipalityAsync();
    var muniId = GetCurrentMunicipalityId();

    if (endDate.HasValue) endDate = endDate.Value.Date.AddDays(1).AddTicks(-1);

    // All teacher IDs that belong to schools in THIS municipality
    var teacherIdsInMuni = _context.Teachers.AsNoTracking()
        .Where(t => _context.Schools.Any(sc => sc.Id == t.SchoolId && sc.AdminId == muniId))
        .Select(t => t.Id.ToString());

    // Base query = only salaries for those teachers
    IQueryable<Salary> q = _context.Salaries.AsNoTracking()
        .Where(s => teacherIdsInMuni.Contains(s.TeacherId.Trim()));

    // Optional filters
    if (schoolId.HasValue && schoolId.Value > 0)
    {
        q = q.Join(_context.Teachers, s => s.TeacherId.Trim(), t => t.Id.ToString(), (s, t) => new { s, t })
             .Where(x => x.t.SchoolId == schoolId.Value)
             .Select(x => x.s);
    }

    if (teacherId.HasValue && teacherId.Value > 0)
        q = q.Where(s => s.TeacherId.Trim() == teacherId.Value.ToString());

    if (startDate.HasValue) q = q.Where(s => s.Date >= startDate.Value);
    if (endDate.HasValue)   q = q.Where(s => s.Date <= endDate.Value);

    var vm = await q.OrderByDescending(s => s.Id)
        .Select(s => new SalaryAcknowledgmentViewModel
        {
            Id                  = s.Id,
            FullName            = s.TeacherName,
            IssuedDate          = s.Date,
            BasicSalary         = s.BasicSalary,
            PF                  = s.EmployeesProvidentFund,
            CIT                 = s.CitizenInvestmentTrust,
            Dearness            = s.DearnessAllowance,
            HeadmasterAllowance = s.HeadmasterAllowance,
            Clothing            = s.ClothingAllowance,
            Festival            = s.FestivalAllowance,
            TotalSalary         = s.TotalSalary,
            Status              = s.Status
        })
        .ToListAsync();

    ViewBag.SelectedSchoolId  = schoolId;
    ViewBag.SelectedTeacherId = teacherId;
    ViewBag.StartDate         = startDate?.ToString("yyyy-MM-dd");
    ViewBag.EndDate           = endDate?.ToString("yyyy-MM-dd");

    return View("~/Views/Admin/SalaryAcknowledgment/SalaryList.cshtml", vm);
}

        // -------------------- List --------------------

        [HttpGet]
        public async Task<IActionResult> AckSalaryList()
{
    var muniId = GetCurrentMunicipalityId();  // <- helper from earlier

    // All Teacher IDs that belong to this municipality (via School)
    var teacherIdsInMuni = _context.Teachers
        .AsNoTracking()
        .Where(t => _context.Schools.Any(sc => sc.Id == t.SchoolId && sc.AdminId == muniId))
        .Select(t => t.Id.ToString());

    // Now filter salaries to only those teacher IDs (trim in case of trailing spaces)
    var vm = await _context.Salaries.AsNoTracking()
        .Where(s => teacherIdsInMuni.Contains(s.TeacherId.Trim()))
        .OrderByDescending(s => s.Id)
        .Select(s => new SalaryAcknowledgmentViewModel
        {
            Id                  = s.Id,
            FullName            = s.TeacherName,
            IssuedDate          = s.Date,
            BasicSalary         = s.BasicSalary,
            PF                  = s.EmployeesProvidentFund,
            CIT                 = s.CitizenInvestmentTrust,
            Dearness            = s.DearnessAllowance,
            HeadmasterAllowance = s.HeadmasterAllowance,
            Clothing            = s.ClothingAllowance,
            Festival            = s.FestivalAllowance,
            TotalSalary         = s.TotalSalary,
            Status              = s.Status
        })
        .ToListAsync();

    return View("~/Views/Admin/SalaryAcknowledgment/SalaryList.cshtml", vm);
}
[HttpGet]
public async Task<IActionResult> Pdf(int id)
{
    var s = await _context.Salaries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    if (s == null) return NotFound();

    // Resolve teacher & school
    int teacherId = int.TryParse(s.TeacherId, out var tmp) ? tmp : 0;
    var t = await _context.Teachers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == teacherId);
    var school = (t != null)
        ? await _context.Schools.AsNoTracking().FirstOrDefaultAsync(sc => sc.Id == t.SchoolId)
        : null;

    // Compute optional amounts (adjust to your rules or stored values)
    var clothingAmt = s.ClothingAllowance ? 10000m : 0m;             // example rule
    var festivalAmt = s.FestivalAllowance ? (s.BasicSalary + s.GradeAmount) : 0m; // example rule

    var totalAllow = s.TotalAllowance > 0 ? s.TotalAllowance
                    : s.GradeAmount + s.EmployeesProvidentFund + s.CitizenInvestmentTrust
                      + s.DearnessAllowance + s.HeadmasterAllowance
                      + clothingAmt + festivalAmt;

    var totalSalary = s.TotalSalary > 0 ? s.TotalSalary : s.BasicSalary + totalAllow;

    var vm = new SalaryPdfViewModel
    {
        Id = s.Id,
        SchoolName = school?.SchoolName ?? "School",
        SchoolAddress = school?.Tole ?? "", // adjust your field name
        TeacherName = s.TeacherName ?? (t != null ? $"{t.FirstName} {(string.IsNullOrEmpty(t.MiddleName) ? "" : t.MiddleName + " ")}{t.LastName}" : "Teacher"),
        IssuedDate = s.Date,
        Status = s.Status,

        BasicSalary = s.BasicSalary,
        GradeAmount = s.GradeAmount,
        PF = s.EmployeesProvidentFund,
        CIT = s.CitizenInvestmentTrust,
        Dearness = s.DearnessAllowance,
        HeadmasterAllowance = s.HeadmasterAllowance,
        ClothingAmount = clothingAmt,
        FestivalAmount = festivalAmt,

        TotalAllowance = totalAllow,
        TotalSalary = totalSalary,

        ReferenceNo = $"LTR-{s.Id:00000}",
        FiscalYear = $"{DateTime.Now.Year}-{DateTime.Now.Year + 1}"
    };

    return new ViewAsPdf("~/Views/School/Salary/Pdf.cshtml", vm)
    {
        PageSize = Size.A4,
        PageOrientation = Orientation.Portrait,
        PageMargins = new Margins(10, 10, 10, 10)
        // no FileName => browser opens inline; user can click download
    };
}
        // -------------------- Edit --------------------

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var s = await _context.Salaries.FindAsync(id);
            if (s == null) return NotFound();

            await LoadTeachersForCurrentSchoolAsync(); // to re-populate dropdown in edit view
            return View("~/Views/School/Salary/TeacherSalaryCalculation.cshtml", s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Salary model)
        {
            if (!ModelState.IsValid)
            {
                await LoadTeachersForCurrentSchoolAsync();
                return View("~/Views/School/Salary/TeacherSalaryCalculation.cshtml", model);
            }

            var s = await _context.Salaries.FindAsync(model.Id);
            if (s == null) return NotFound();

            var schoolId = GetCurrentSchoolId();

            // If TeacherId changed (or even if same, we can refresh snapshot)
            if (s.TeacherId != model.TeacherId)
            {
                var fullName = await GetTeacherFullNameAsync(int.Parse(model.TeacherId), schoolId);


                if (string.IsNullOrEmpty(fullName))
                {
                    ModelState.AddModelError("TeacherId", "Selected teacher is not in your school.");
                    await LoadTeachersForCurrentSchoolAsync();
                    return View("~/Views/School/Salary/TeacherSalaryCalculation.cshtml", model);
                }
                s.TeacherId = model.TeacherId;
                s.TeacherName = fullName;
            }
            else
            {
                // Keep existing snapshot or refresh it if you prefer
                s.TeacherName = s.TeacherName; // no change
            }

            // Copy the rest of the editable fields
            s.AppointmentType = model.AppointmentType;
            s.Level = model.Level;
            s.Category = model.Category;
            s.Grade = model.Grade;
            s.Date = model.Date;
            s.BasicSalary = model.BasicSalary;
            s.GradeAmount = model.GradeAmount;
            s.EmployeesProvidentFund = model.EmployeesProvidentFund;
            s.CitizenInvestmentTrust = model.CitizenInvestmentTrust;
            s.DearnessAllowance = model.DearnessAllowance;
            s.HeadmasterAllowance = model.HeadmasterAllowance;
            s.FestivalAllowance = model.FestivalAllowance;
            s.ClothingAllowance = model.ClothingAllowance;

            // As requested: NOT storing totals here.
            // If later needed: s.TotalAllowance = ...; s.TotalSalary = ...;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Salary updated.";
            return RedirectToAction(nameof(SalaryList));
        }

        // -------------------- Delete --------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _context.Salaries.FindAsync(id);
            if (s != null)
            {
                _context.Salaries.Remove(s);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Salary deleted.";
            }
            return RedirectToAction(nameof(SalaryList));
        }

        // -------------------- PDF (Empty for now) --------------------

        [HttpGet]
        public async Task<IActionResult> GeneratePDF(int id)
        {
            var s = await _context.Salaries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            using var ms = new MemoryStream();
            using (var doc = new Document(PageSize.A4, 36, 36, 36, 36))
            {
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Blank/placeholder PDF (valid)
                doc.NewPage();

                doc.Close();
            }

            var fileName = $"SalarySlip_{s.TeacherName}_{s.Id}.pdf";
            return File(ms.ToArray(), "application/pdf", fileName);
        }
    }
}

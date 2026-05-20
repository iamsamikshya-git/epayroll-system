using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using E_PayRoll.Data;
using E_PayRoll.Models;
using E_PayRoll.Helpers;
using E_PayRoll.ViewModels;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace E_PayRoll.Controllers
{
    [Authorize(Roles = "School")]
    public class SchoolController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SchoolController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
{
    var schoolId = GetCurrentSchoolId(); // you already have this helper
    var q = _context.Teachers.Where(t => t.SchoolId == schoolId);

    // Core counts
    ViewBag.TotalTeachers = q.Count();
    ViewBag.MaleTeachers  = q.Count(t => t.Gender != null && t.Gender.Trim().ToLower() == "male");
    ViewBag.FemaleTeachers= q.Count(t => t.Gender != null && t.Gender.Trim().ToLower() == "female");
    ViewBag.ActiveTeachers= q.Count(t => t.TeacherStatus != null && t.TeacherStatus.Trim().ToLower() == "working");

    // Level counts (use LIKE to be tolerant of names like "Primary Level")
    ViewBag.PrimaryLevelTeachers = q.Count(t => t.TeacherLevel != null &&
                                                EF.Functions.Like(t.TeacherLevel, "%Primary%"));
    ViewBag.SecondaryLevelTeachers = q.Count(t => t.TeacherLevel != null &&
                                                  EF.Functions.Like(t.TeacherLevel, "%Secondary%"));

    return View();
}

        private int GetCurrentSchoolId()
{
    var username = User.Identity?.Name;
    var school = _context.Schools
        .Include(a => a.User)
        .FirstOrDefault(a => a.User != null && a.User.Username == username);

    if (school == null) throw new InvalidOperationException("Logged-in school not found.");
    return school.Id;
}

        

        // STEP 1: Basic Info
        [HttpGet]
        public IActionResult BasicInfo()
        {
            var sessionModel = HttpContext.Session.GetObject<Teacher>("Teacher") ?? new Teacher
            {
                SchoolName = GetLoggedInSchoolName(),
                Type = "Teacher"
            };

            var viewModel = new TeacherBasicInfoViewModel
            {
                SchoolName = sessionModel.SchoolName,
                Type = sessionModel.Type,
                SchoolCode = sessionModel.SchoolCode,
                FirstName = sessionModel.FirstName,
                MiddleName = sessionModel.MiddleName,
                LastName = sessionModel.LastName,
                Gender = sessionModel.Gender,
                DateOfBirth = sessionModel.DateOfBirth,
                PermanentCountry = sessionModel.PermanentCountry,
                PermanentProvince = sessionModel.PermanentProvince,
                PermanentDistrict = sessionModel.PermanentDistrict,
                PermanentLocalLevel = sessionModel.PermanentLocalLevel,
                PermanentWard = sessionModel.PermanentWard,
                PermanentTole = sessionModel.PermanentTole,
                TemporaryCountry = sessionModel.TemporaryCountry,
                TemporaryProvince = sessionModel.TemporaryProvince,
                TemporaryDistrict = sessionModel.TemporaryDistrict,
                TemporaryLocalLevel = sessionModel.TemporaryLocalLevel,
                TemporaryWard = sessionModel.TemporaryWard,
                TemporaryTole = sessionModel.TemporaryTole
            };

            return View("Teacher/BasicInfo", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BasicInfo(TeacherBasicInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Field: {entry.Key} - Error: {error.ErrorMessage}");
                    }
                }
                return View("Teacher/BasicInfo", model);
            }


            var teacher = HttpContext.Session.GetObject<Teacher>("Teacher") ?? new Teacher();
            // 🔁 Preserve existing Id if it's already set (edit mode)
            teacher.Id = teacher.Id > 0 ? teacher.Id : model.Id;
            teacher.SchoolName = model.SchoolName;
            teacher.Type = model.Type;
            teacher.SchoolCode = model.SchoolCode;
            teacher.FirstName = model.FirstName;
            teacher.MiddleName = model.MiddleName ?? string.Empty;
            teacher.LastName = model.LastName;
            teacher.Gender = model.Gender;
            teacher.DateOfBirth = model.DateOfBirth;

            teacher.PermanentCountry = _context.Countries
                .FirstOrDefault(c => c.Id == Convert.ToInt32(model.PermanentCountry))?.Name;

            teacher.PermanentProvince = _context.Provinces
                .FirstOrDefault(p => p.Id == Convert.ToInt32(model.PermanentProvince))?.Name;

            teacher.PermanentDistrict = _context.Districts
                .FirstOrDefault(d => d.Id == Convert.ToInt32(model.PermanentDistrict))?.Name;

            teacher.PermanentLocalLevel = _context.Municipalities
                .FirstOrDefault(m => m.Id == Convert.ToInt32(model.PermanentLocalLevel))?.Name;
            teacher.PermanentWard = model.PermanentWard;
            teacher.PermanentTole = model.PermanentTole;

            teacher.TemporaryCountry = _context.Countries
                .FirstOrDefault(c => c.Id == Convert.ToInt32(model.TemporaryCountry))?.Name;

            teacher.TemporaryProvince = _context.Provinces
                .FirstOrDefault(p => p.Id == Convert.ToInt32(model.TemporaryProvince))?.Name;

            teacher.TemporaryDistrict = _context.Districts
                .FirstOrDefault(d => d.Id == Convert.ToInt32(model.TemporaryDistrict))?.Name;

            teacher.TemporaryLocalLevel = _context.Municipalities
                .FirstOrDefault(m => m.Id == Convert.ToInt32(model.TemporaryLocalLevel))?.Name;
            teacher.TemporaryWard = model.TemporaryWard;
            teacher.TemporaryTole = model.TemporaryTole;

            HttpContext.Session.SetObject("Teacher", teacher);
            return RedirectToAction("PersonalInfo");
        }

        // STEP 2: Personal Info
        [HttpGet]
        public IActionResult PersonalInfo()
        {
            var teacher = HttpContext.Session.GetObject<Teacher>("Teacher");
            if (teacher == null) return RedirectToAction("BasicInfo");

            var viewModel = new TeacherPersonalInfoViewModel
            {
                FatherName = teacher.FatherName,
                MotherName = teacher.MotherName,
                SpouseName = teacher.SpouseName,
                Ethnicity = teacher.Ethnicity,
                MotherTongue = teacher.MotherTongue,
                Disability = teacher.Disability,
                Email = teacher.Email,
                Contact = teacher.Contact,
                CitizenshipNo = teacher.CitizenshipNo,
                CitizenshipIssueDate = teacher.CitizenshipIssueDate ?? default(DateTime),
                CitizenshipIssueDistrict = teacher.CitizenshipIssueDistrict,
                NationalId = teacher.NationalId,
                PermanentAccount = teacher.PermanentAccount,
                UnionFundNo = teacher.UnionFundNo,
                InvestmentFundNo = teacher.InvestmentFundNo,
                BankName = teacher.BankName,
                BankAccount = teacher.BankAccount,
                LicenseNo = teacher.LicenseNo,
                ContributionFund = teacher.ContributionFund,
                SheetRoll = teacher.SheetRoll,
                // Populate dropdowns
                TeacherLevels = _context.TeacherLevels
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
            .ToList(),

                TeacherCategories = _context.TeacherCategories
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToList(),

                AppointmentTypes = _context.AppointmentTypes
            .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
            .ToList(),
                DistrictList = _context.Districts
            .Select(d => new SelectListItem
            {
                Value = d.Name,  // Or use d.Id.ToString() if you store Id
                Text = d.Name
            }).ToList()
            };

            return View("Teacher/PersonalInfo", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PersonalInfo(TeacherPersonalInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"❌ Field: {entry.Key} | Error: {error.ErrorMessage}");
                    }
                }
                // Repopulate dropdowns
                model.TeacherLevels = _context.TeacherLevels
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
            .ToList();

                model.TeacherCategories = _context.TeacherCategories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();

                model.AppointmentTypes = _context.AppointmentTypes
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                    .ToList();
                // Repopulate districts
                model.DistrictList = _context.Districts
                    .Select(d => new SelectListItem
                    {
                        Value = d.Name,
                        Text = d.Name
                    }).ToList();

                return View("Teacher/PersonalInfo", model);
            }


            var teacher = HttpContext.Session.GetObject<Teacher>("Teacher") ?? new Teacher();
            teacher.Id = teacher.Id > 0 ? teacher.Id : model.Id;


            teacher.FatherName = model.FatherName;
            teacher.MotherName = model.MotherName;
            teacher.SpouseName = model.SpouseName;
            teacher.Ethnicity = model.Ethnicity;
            teacher.MotherTongue = model.MotherTongue;
            teacher.Disability = model.Disability;
            teacher.Email = model.Email;
            // Handle photo upload (IFormFile -> byte[])
            if (model.Photo != null && model.Photo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    model.Photo.CopyTo(ms);
                    teacher.Photo = ms.ToArray();
                }
            }
            teacher.Contact = model.Contact;
            teacher.CitizenshipNo = model.CitizenshipNo;
            teacher.CitizenshipIssueDate = model.CitizenshipIssueDate;
            teacher.CitizenshipIssueDistrict = model.CitizenshipIssueDistrict;
            teacher.NationalId = model.NationalId;
            teacher.PermanentAccount = model.PermanentAccount;
            teacher.UnionFundNo = model.UnionFundNo;
            teacher.InvestmentFundNo = model.InvestmentFundNo;
            teacher.BankName = model.BankName;
            teacher.BankAccount = model.BankAccount;
            teacher.LicenseNo = model.LicenseNo;
            teacher.ContributionFund = model.ContributionFund;
            teacher.SheetRoll = model.SheetRoll;
           teacher.TeacherLevel = _context.TeacherLevels
    .FirstOrDefault(l => l.Id == Convert.ToInt32(model.TeacherLevel))?.Name;

teacher.TeacherCategory = _context.TeacherCategories
    .FirstOrDefault(c => c.Id == Convert.ToInt32(model.TeacherCategory))?.Name;

teacher.AppointmentType = _context.AppointmentTypes
    .FirstOrDefault(a => a.Id == Convert.ToInt32(model.AppointmentType))?.Name;
            teacher.AppointmentStatus = model.AppointmentStatus;
            teacher.TeacherStatus = model.TeacherStatus;

            HttpContext.Session.SetObject("Teacher", teacher);
            return RedirectToAction("ExperienceInfo");
        }

        // STEP 3: Experience
        [HttpGet]
        public IActionResult ExperienceInfo()
        {
            var teacher = HttpContext.Session.GetObject<Teacher>("Teacher");
            if (teacher == null) return RedirectToAction("PersonalInfo");

            var viewModel = new TeacherExperienceViewModel
            {

                EducationLevel = teacher.EducationLevel,
                BoardUniversity = teacher.BoardUniversity,
                PassedYear = teacher.PassedYear,
                Faculty = teacher.Faculty,
                GradeGPA = teacher.GradeGPA,
                PostType = teacher.PostType,
                AppointmentStartDate = teacher.AppointmentStartDate,
                DecisionDate = teacher.DecisionDate,
                AttendanceDate = teacher.AttendanceDate,
                AppointedSchool = teacher.AppointedSchool,
                SchoolAddress = teacher.SchoolAddress,
                AppointedSubject = teacher.AppointedSubject
            };

            return View("Teacher/ExperienceInfo", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExperienceInfo(TeacherExperienceViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Teacher/ExperienceInfo", model);

            var teacher = HttpContext.Session.GetObject<Teacher>("Teacher") ?? new Teacher();
            teacher.Id = teacher.Id > 0 ? teacher.Id : model.Id;




            teacher.EducationLevel = model.EducationLevel;
            teacher.BoardUniversity = model.BoardUniversity;
            teacher.PassedYear = model.PassedYear;
            teacher.Faculty = model.Faculty;
            teacher.GradeGPA = model.GradeGPA;

            teacher.PostType = model.PostType;
            teacher.AppointmentStartDate = model.AppointmentStartDate;
            teacher.DecisionDate = model.DecisionDate;
            teacher.AttendanceDate = model.AttendanceDate;
            teacher.AppointedSchool = model.AppointedSchool;
            teacher.SchoolAddress = model.SchoolAddress;
            teacher.AppointedSubject = model.AppointedSubject;

            // ✅ make sure SchoolId is set for new teachers (and preserved for edits)
    teacher.SchoolId = teacher.SchoolId > 0 ? teacher.SchoolId : GetCurrentSchoolId();

            teacher.SchoolName = GetLoggedInSchoolName();
            if (teacher.Id > 0)
            {
                // Update existing
                _context.Teachers.Update(teacher);
            }
            else
            {
                // Insert new
                _context.Teachers.Add(teacher);
            }


            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Teacher");

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View("Teacher/Success");
        }

        public IActionResult TeacherList()
        {
            var schooId = GetCurrentSchoolId();
            var teachers = _context.Teachers.Where(t => t.SchoolId == schooId).ToList();
            return View("Teacher/TeacherList", teachers);
        }
        //GET: /School/TeacherProfile/5
    [HttpGet]
    public async Task<IActionResult> TeacherProfile(int id)
    {
        var teacher = await _context.Teachers
            // If you later add navigation props, include them here
            // .Include(t => t.TeacherLevelEntity)
            // .Include(t => t.TeacherCategoryEntity)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (teacher == null) return NotFound();
        return View("Teacher/TeacherProfile", teacher);
    }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var teacher = _context.Teachers.FirstOrDefault(t => t.Id == id);
            if (teacher == null)
                return NotFound();

            // Store the existing teacher in session to edit through the multi-step form
            HttpContext.Session.SetObject("Teacher", teacher);

            return RedirectToAction("BasicInfo");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return RedirectToAction("TeacherList");
        }


        [HttpGet]
        public JsonResult GetCountries()
        {
            var countries = _context.Countries
                .Select(c => new { id = c.Id, name = c.Name })
                .ToList();
            return Json(countries);
        }

        [HttpGet]
        public JsonResult GetProvinces(int countryId)
        {
            var provinces = _context.Provinces
                .Where(p => p.CountryId == countryId)
                .Select(p => new { id = p.Id, name = p.Name })
                .ToList();
            return Json(provinces);
        }

        [HttpGet]
        public JsonResult GetDistricts(int provinceId)
        {
            var districts = _context.Districts
                .Where(d => d.ProvinceId == provinceId)
                .Select(d => new { id = d.Id, name = d.Name })
                .ToList();
            return Json(districts);
        }

        [HttpGet]
        public JsonResult GetMunicipalities(int districtId)
        {
            var municipalities = _context.Municipalities
                .Where(m => m.DistrictId == districtId)
                .Select(m => new { id = m.Id, name = m.Name })
                .ToList();
            return Json(municipalities);
        }
        //for teacher level
        [HttpGet]
        public IActionResult AddTeacherLevel()
        {
            return View("Setting/AddTeacherLevel");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeacherLevel(TeacherLevel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.TeacherLevels.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("TeacherLevelList");
        }
        //for teacher level list
        public IActionResult TeacherLevelList()
        {
            var levels = _context.TeacherLevels.ToList();
            return View("Setting/TeacherLevelList", levels); // This looks for Views/Setting/TeacherLevelList.cshtml
        }
        // GET: Edit
        public IActionResult EditTeacherLevel(int id)
        {
            var level = _context.TeacherLevels.Find(id);
            if (level == null) return NotFound();
            return View("AddTeacherLevel", level); // Reuse the same view for editing
        }

        // POST: Delete
        [HttpPost]
        public IActionResult DeleteTeacherLevel(int id)
        {
            var level = _context.TeacherLevels.Find(id);
            if (level != null)
            {
                _context.TeacherLevels.Remove(level);
                _context.SaveChanges();
            }
            return RedirectToAction("TeacherLevelList");
        }

        // GET: Add Teacher Category (Form)
        [HttpGet]
        public IActionResult AddTeacherCategory()
        {
            return View("Setting/AddTeacherCategory");
        }

        // POST: Add Teacher Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeacherCategory(TeacherCategory model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.TeacherCategories.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("TeacherCategoryList");
        }

        // GET: Teacher Category List
        [HttpGet]
        public IActionResult TeacherCategoryList()
        {
            var categories = _context.TeacherCategories.ToList();
            return View("Setting/TeacherCategoryList", categories); // This looks for Views/Setting/TeacherCategoryList.cshtml
        }

        // GET: Edit Teacher Category
        [HttpGet]
        public IActionResult EditTeacherCategory(int id)
        {
            var category = _context.TeacherCategories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Edit Teacher Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeacherCategory(TeacherCategory model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.TeacherCategories.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("TeacherCategoryList");
        }

        // POST: Delete Teacher Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeacherCategory(int id)
        {
            var category = await _context.TeacherCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.TeacherCategories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("TeacherCategoryList");
        }


        // GET: /Salary/AddBasicSalary
        [HttpGet]
        public IActionResult AddBasicSalary()
        {
            var viewModel = new BasicSalaryViewModel
            {
                Levels = _context.TeacherLevels
                    .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
                    .ToList(),

                Categories = _context.TeacherCategories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList()

            };

            return View("Setting/AddBasicSalary", viewModel);

        }

        // POST: /Salary/AddBasicSalary
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBasicSalary(BasicSalaryViewModel model)
        {
            // if (!ModelState.IsValid)
            // {
            //     // Re-populate dropdowns before returning the view
            //     model.Levels = _context.TeacherLevels
            //         .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
            //         .ToList();

            //         model.Categories = _context.TeacherCategories
            //             .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            //             .ToList();
            //         //model.AppointmentTypes = _context.AppointmentTypes.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList();

            //     return View("Setting/AddBasicSalary", model); 

            // }

            var entity = new BasicSalary
            {
                TeacherLevelId = model.TeacherLevelId,
                TeacherCategoryId = model.TeacherCategoryId,

                BaseSalaryAmount = model.BaseSalaryAmount
            };

            _context.BasicSalaries.Add(entity);
            _context.SaveChanges();

            return RedirectToAction("BasicSalaryList", "School"); // You can create this view later
        }
        // List all salaries
        public IActionResult BasicSalaryList()
        {
            var salaries = _context.BasicSalaries
                .Include(b => b.TeacherLevel)
                .Include(b => b.TeacherCategory)
                .ToList();
            return View("Setting/BasicSalaryList", salaries); // This looks for Views/Setting/BasicSalaryList.cshtml

        }
        //GET: Edit salary
        public IActionResult EditBasicSalary(int id)
        {
            var salary = _context.BasicSalaries.Find(id);
            if (salary == null) return NotFound();

            var viewModel = new BasicSalaryViewModel
            {
                Id = salary.Id,
                TeacherLevelId = salary.TeacherLevelId,
                TeacherCategoryId = salary.TeacherCategoryId,
                BaseSalaryAmount = salary.BaseSalaryAmount,
                Levels = _context.TeacherLevels.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList(),
                Categories = _context.TeacherCategories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Edit salary
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditBasicSalary(BasicSalaryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Levels = _context.TeacherLevels.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList();

                model.Categories = _context.TeacherCategories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                return View(model);
            }

            var salary = _context.BasicSalaries.Find(model.Id);
            if (salary == null) return NotFound();

            salary.TeacherLevelId = model.TeacherLevelId;
            salary.TeacherCategoryId = model.TeacherCategoryId;
            salary.BaseSalaryAmount = model.BaseSalaryAmount;

            _context.SaveChanges();

            return RedirectToAction("BasicSalaryList");
        }

        // POST: Delete salary
        [HttpPost]
        public IActionResult DeleteBasicSalary(int id)
        {
            var salary = _context.BasicSalaries.Find(id);
            if (salary == null) return NotFound();

            _context.BasicSalaries.Remove(salary);
            _context.SaveChanges();

            return RedirectToAction("BasicSalaryList");
        }
        // GET: Add Appointment Type
        public IActionResult AddAppointmentType()
        {
            return View("Setting/AddAppointmentType");
        }

        // POST: Add Appointment Type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAppointmentType(AppointmentType model)
        {
            if (ModelState.IsValid)
            {
                _context.AppointmentTypes.Add(model);
                _context.SaveChanges();
                return RedirectToAction("AppointmentTypeList");
            }

            return View(model);
        }

        // GET: Appointment Type List
        public IActionResult AppointmentTypeList()
        {
            var list = _context.AppointmentTypes.ToList();
            return View("Setting/AppointmentTypeList", list); // This looks for Views/Setting/AppointmentTypeList.cshtml
        }

        // GET: Edit Appointment Type
        public IActionResult EditAppointmentType(int id)
        {
            var item = _context.AppointmentTypes.FirstOrDefault(a => a.Id == id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: Edit Appointment Type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAppointmentType(AppointmentType model)
        {
            if (ModelState.IsValid)
            {
                _context.AppointmentTypes.Update(model);
                _context.SaveChanges();
                return RedirectToAction("AppointmentTypeList");
            }

            return View(model);
        }

        // POST: Delete Appointment Type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAppointmentType(int id)
        {
            var item = _context.AppointmentTypes.FirstOrDefault(a => a.Id == id);
            if (item != null)
            {
                _context.AppointmentTypes.Remove(item);
                _context.SaveChanges();
            }

            return RedirectToAction("AppointmentTypeList");
        }

        // GET: AddGradeNumber
        [HttpGet]
        public IActionResult AddGradeNumber()
        {
            return View("Setting/AddGradeNumber");
        }

        // POST: AddGradeNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddGradeNumber(GradeNumber model)
        {
            if (ModelState.IsValid)
            {
                _context.GradeNumbers.Add(model);
                _context.SaveChanges();
                return RedirectToAction("GradeNumberList");
            }

            return View(model);
        }

        // GET: GradeNumberList
        public IActionResult GradeNumberList()
        {
            var grades = _context.GradeNumbers.ToList();
            return View("Setting/GradeNumberList", grades); // This looks for Views/Setting/GradeNumberList.cshtml
        }

        // GET: Edit
        public IActionResult EditGradeNumber(int id)
        {
            var grade = _context.GradeNumbers.Find(id);
            if (grade == null)
                return NotFound();

            return View(grade);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGradeNumber(GradeNumber model)
        {
            if (ModelState.IsValid)
            {
                _context.GradeNumbers.Update(model);
                _context.SaveChanges();
                return RedirectToAction("GradeNumberList");
            }

            return View(model);
        }

        // POST: Delete
        [HttpPost]
        public IActionResult DeleteGradeNumber(int id)
        {
            var grade = _context.GradeNumbers.Find(id);
            if (grade != null)
            {
                _context.GradeNumbers.Remove(grade);
                _context.SaveChanges();
            }
            return RedirectToAction("GradeNumberList");
        }
        private string GetLoggedInSchoolName()
        {
            var username = User.Identity?.Name;

            var school = _context.Schools
                .Include(a => a.User)
                .FirstOrDefault(a => a.User != null && a.User.Username == username);

            return school?.SchoolName ?? string.Empty;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_PayRoll.Data;
using E_PayRoll.Models;
using E_PayRoll.ViewModels;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using E_PayRollAdminListViewModel.Models;
using AdminListViewModel = E_PayRollAdminListViewModel.Models.AdminListViewModel;
using Microsoft.EntityFrameworkCore;
using E_PayRoll.ViewModels; // Ensure this is the correct namespace for AdminEditViewModel
using Microsoft.AspNetCore.Mvc.Rendering;


// … your usings …
namespace E_PayRoll.Controllers
{
    [Authorize]
    public class SuperAdminController : Controller
    {
         [Authorize(Roles = "SuperAdmin")] // optional role restriction
    public IActionResult Dashboard()
{
    ViewBag.AdminCount = _context.Admins.Count();

    // Count Rural Municipalities
    ViewBag.RuralMunicipalityCount = _context.Municipalities
        .Count(m => m.Name.Contains("Rural"));

    // Count Municipalities (everything else)
    ViewBag.MunicipalityCount = _context.Municipalities.Count()
        - ViewBag.RuralMunicipalityCount;

    return View();
}

        private readonly ApplicationDbContext _context;
        public SuperAdminController(ApplicationDbContext context) => _context = context;

        /* ---------------- EXISTING LOGIN / LOGOUT / DASHBOARD --------------- */
        // (unchanged code – as you already have)

        /* ---------------- 1.  CREATE-ADMIN (GET)  --------------------------- */
        // just returns the Razor view;  all dropdowns are filled by AJAX
        [HttpGet]
public IActionResult CreateAdmin()
{
    return View();
}
        [HttpPost]
        public IActionResult CreateAdmin(AdminCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("", "Username already exists");
                return View(model);
            }

            // Step 1: Create User
            var user = new User
            {
                Username = model.Username,
                Password = model.Password, // 🔐 Hash this in production
                Role = "Admin"
            };
            _context.Users.Add(user);
            _context.SaveChanges(); // Saves and gets User.Id
                                    // Save profile picture (returns /uploads/filename.jpg or null)
            var picturePath = SaveProfilePicture(model.ProfilePicture);

            // Step 2: Create Admin linked to User
            var admin = new Admin
            {
                Email = model.Email,
                CountryId = model.CountryId,
                ProvinceId = model.ProvinceId,
                DistrictId = model.DistrictId,
                MunicipalityId = model.MunicipalityId,
                ProfilePicture = SaveProfilePicture(model.ProfilePicture),
                UserId = user.Id // link to login credentials
            };
            _context.Admins.Add(admin);
            _context.SaveChanges();

            TempData["Success"] = "Admin created successfully";
            return RedirectToAction("Adminlist");
        }
public IActionResult AdminList()
{
    var admins = _context.Admins
        .Select(a => new AdminListViewModel
        {
            Id = a.Id,
            Email = a.Email,
            ProfilePicture = a.ProfilePicture,
            Username = _context.Users
                        .Where(u => u.Id == a.UserId)
                        .Select(u => u.Username)
                        .FirstOrDefault() ?? "",

            CountryName = _context.Countries
                          .Where(c => c.Id == a.CountryId)
                          .Select(c => c.Name)
                          .FirstOrDefault() ?? "",

            ProvinceName = _context.Provinces
                          .Where(p => p.Id == a.ProvinceId)
                          .Select(p => p.Name)
                          .FirstOrDefault() ?? "",

            DistrictName = _context.Districts
                          .Where(d => d.Id == a.DistrictId)
                          .Select(d => d.Name)
                          .FirstOrDefault() ?? "",

            MunicipalityName = _context.Municipalities
                          .Where(m => m.Id == a.MunicipalityId)
                          .Select(m => m.Name)
                          .FirstOrDefault() ?? ""
        })
        .ToList();

    return View(admins);
}
 /* ---------------- ADMIN PROFILE (optional) ---------------- */
 [Authorize(Roles = "SuperAdmin,Admin")]
[HttpGet]
public async Task<IActionResult> AdminProfile(int id)
{
    var admin = await _context.Admins
        .Include(a => a.User)
        .Include(a => a.Country)
        .Include(a => a.Province)
        .Include(a => a.District)
        .Include(a => a.Municipality)
        .FirstOrDefaultAsync(a => a.Id == id);

    if (admin == null) return NotFound();

    // If a normal Admin is viewing, enforce "self-only"
    if (User.IsInRole("Admin"))
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username)) return Unauthorized();

        var currentUser = await _context.Users
            .Where(u => u.Username == username)
            .Select(u => new { u.Id })
            .FirstOrDefaultAsync();

        if (currentUser == null || admin.UserId != currentUser.Id)
            return Forbid(); // block viewing other admins' profiles
    }

    // If you keep the view at Views/SuperAdmin/AdminProfile.cshtml:
    return View("AdminProfile", admin);
}

        //GET: SuperAdmin/EditAdmin/5 -> returns CreateAdmin view with data filled
[HttpGet]
public async Task<IActionResult> EditAdmin(int id)
{
    var admin = await _context.Admins
        .Include(a => a.User)
        .FirstOrDefaultAsync(a => a.Id == id);

    if (admin == null)
    {
        TempData["ErrorMessage"] = "Admin not found.";
        return RedirectToAction("AdminList");
    }

    var vm = new AdminCreateViewModel
    {
        Id = admin.Id,
        Username = admin.User.Username,
        Email = admin.Email,
        CountryId = admin.CountryId,
        ProvinceId = admin.ProvinceId,
        DistrictId = admin.DistrictId,
        MunicipalityId = admin.MunicipalityId,
        ExistingProfilePicturePath = admin.ProfilePicture
        // Password fields left blank intentionally
    };

    await PopulateCreateAdminSelects(vm.CountryId, vm.ProvinceId, vm.DistrictId, vm.MunicipalityId);
    return View("CreateAdmin", vm); // reuse the same view
}

        // POST: SuperAdmin/EditAdmin  (same ViewModel as Create)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(AdminCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCreateAdminSelects(model.CountryId, model.ProvinceId, model.DistrictId, model.MunicipalityId);
                return View("CreateAdmin", model);
            }

            var admin = await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == model.Id);

            if (admin == null)
            {
                TempData["ErrorMessage"] = "Admin not found.";
                return RedirectToAction("AdminList");
            }

            // Username uniqueness if changed
            if (!string.Equals(admin.User.Username, model.Username, StringComparison.OrdinalIgnoreCase))
            {
                var taken = await _context.Users.AnyAsync(u => u.Username == model.Username);
                if (taken)
                {
                    ModelState.AddModelError(nameof(model.Username), "Username already exists.");
                    await PopulateCreateAdminSelects(model.CountryId, model.ProvinceId, model.DistrictId, model.MunicipalityId);
                    return View("CreateAdmin", model);
                }
                admin.User.Username = model.Username;
            }

            // Update core fields
            admin.Email = model.Email;
            admin.CountryId = model.CountryId;
            admin.ProvinceId = model.ProvinceId;
            admin.DistrictId = model.DistrictId;
            admin.MunicipalityId = model.MunicipalityId;

            // Update password only if provided & matches
            if (!string.IsNullOrWhiteSpace(model.Password) || !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                if (model.Password == model.ConfirmPassword)
                {
                    // TODO: hash in production
                    admin.User.Password = model.Password;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.ConfirmPassword), "Passwords do not match.");
                    await PopulateCreateAdminSelects(model.CountryId, model.ProvinceId, model.DistrictId, model.MunicipalityId);
                    return View("CreateAdmin", model);
                }
            }

            // Replace profile picture if a new one is uploaded
            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                admin.ProfilePicture = SaveProfilePicture(model.ProfilePicture);
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Admin updated successfully!";
                return RedirectToAction("AdminList");
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Unable to save changes. Try again later.";
                await PopulateCreateAdminSelects(model.CountryId, model.ProvinceId, model.DistrictId, model.MunicipalityId);
                return View("CreateAdmin", model);
            }
        }
        



// Helper to fill dropdowns for both Create and Edit on the same view
private async Task PopulateCreateAdminSelects(int countryId, int provinceId, int districtId, int municipalityId)
{
    ViewBag.Countries = new SelectList(await _context.Countries.AsNoTracking().ToListAsync(),
        "Id", "Name", countryId);

    ViewBag.Provinces = new SelectList(await _context.Provinces.AsNoTracking()
        .Where(p => p.CountryId == countryId).ToListAsync(), "Id", "Name", provinceId);

    ViewBag.Districts = new SelectList(await _context.Districts.AsNoTracking()
        .Where(d => d.ProvinceId == provinceId).ToListAsync(), "Id", "Name", districtId);

    ViewBag.Municipalities = new SelectList(await _context.Municipalities.AsNoTracking()
        .Where(m => m.DistrictId == districtId).ToListAsync(), "Id", "Name", municipalityId);
}
        /* ---------------- 3.  JSON END-POINTS FOR DROPDOWNS ----------------- */
        [HttpGet] public JsonResult GetCountries() =>
            Json(_context.Countries.Select(c => new { c.Id, c.Name }));

        [HttpGet] public JsonResult GetProvinces(int countryId) =>
            Json(_context.Provinces
                    .Where(p => p.CountryId == countryId)
                    .Select(p => new { p.Id, p.Name }));

        [HttpGet] public JsonResult GetDistricts(int provinceId) =>
            Json(_context.Districts
                    .Where(d => d.ProvinceId == provinceId)
                    .Select(d => new { d.Id, d.Name }));

        [HttpGet] public JsonResult GetMunicipalities(int districtId) =>
            Json(_context.Municipalities
                    .Where(m => m.DistrictId == districtId)
                    .Select(m => new { m.Id, m.Name }));

        /* ---------------- optional helper to save the uploaded photo -------- */
        private string? SaveProfilePicture(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path     = Path.Combine("wwwroot/uploads", fileName);
            using (var stream = new FileStream(path, FileMode.Create))
                file.CopyTo(stream);
            return "/uploads/" + fileName;
        }
    }
}
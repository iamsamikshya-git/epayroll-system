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


// … your usings …
namespace E_PayRoll.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
         public IActionResult Dashboard()
    {
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
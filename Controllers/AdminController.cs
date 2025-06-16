using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_PayRoll.Data;
using Microsoft.EntityFrameworkCore;
using E_PayRoll.Models;
using E_PayRoll.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace E_PayRoll.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context) => _context = context;

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role == "Admin")
                    return RedirectToAction("Dashboard");
                if (role == "SuperAdmin")
                    return RedirectToAction("Dashboard", "SuperAdmin");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == username && u.Password == password && u.Role == "Admin");

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username!),
                    new Claim(ClaimTypes.Role, user.Role!)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        // GET: Admin/Dashboard
   public IActionResult Dashboard()
{
    var username = User.Identity?.Name;
    var admin = _context.Admins.Include(a => a.User).FirstOrDefault(a => a.User!.Username == username);
    if (admin != null)
    {
        // Only count schools for this admin
        ViewBag.TotalSchools = _context.Schools.Count(s => s.AdminId == admin.Id);
    }
    else
    {
        ViewBag.TotalSchools = 0;
    }
    return View();
}

        // GET: Admin/SchoolList
        public async Task<IActionResult> SchoolList()
        {
            var username = User.Identity?.Name;
            var admin = await _context.Admins.Include(a => a.User).FirstOrDefaultAsync(a => a.User!.Username == username);
            if (admin == null)
            {
                return Unauthorized();
            }

            var schools = await _context.Schools
                .Where(s => s.AdminId == admin.Id)
                .Include(s => s.User)
                .Include(s => s.Admin)
                .ToListAsync();

            var viewModelList = schools.Select(s => new SchoolListViewModel
            {
                School = s,
                User = s.User!,
                Admin = s.Admin
            }).ToList();

            return View(viewModelList);
        }

        // GET: Admin/CreateSchool
        public IActionResult CreateSchool()
        {
            return View(new SchoolListViewModel
            {
                School = new School(),
                User = new User(),
                Admin = new Admin()
            });
        }

        // POST: Admin/CreateSchool
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchool(SchoolListViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Password) ||
                string.IsNullOrWhiteSpace(viewModel.ConfirmPassword) ||
                viewModel.Password != viewModel.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match or are empty.");
            }

            if (_context.Schools.Any(s => s.SchoolName == viewModel.School.SchoolName))
            {
                ModelState.AddModelError("School.SchoolName", "School already exists.");
            }

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key]!.Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation error for {key}: {error.ErrorMessage}");
                    }
                }
                return View(viewModel);
            }

            var user = new User
            {
                Username = viewModel.Username ?? viewModel.School.Email,
                Password = viewModel.Password,
                Role = viewModel.Role ?? "School"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var username = User.Identity?.Name;
            var admin = await _context.Admins.Include(a => a.User).FirstOrDefaultAsync(a => a.User!.Username == username);
            if (admin == null)
            {
                ModelState.AddModelError("", "No admin available to assign.");
                return View(viewModel);
            }

            var school = new School
            {
                SchoolName = viewModel.School.SchoolName,
                Tole = viewModel.School.Tole,
                TelephoneNo = viewModel.School.TelephoneNo,
                WardNo = viewModel.School.WardNo,
                AccountNo = viewModel.School.AccountNo,
                ContactPerson = viewModel.School.ContactPerson,
                Email = viewModel.School.Email,
                UserPerson = viewModel.School.UserPerson,
                EMISCode = viewModel.School.EMISCode,
                UserId = user.Id,
                AdminId = admin.Id
            };

            _context.Schools.Add(school);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "School created successfully!";
            return RedirectToAction(nameof(SchoolList));
        }

        // GET: Admin/EditSchool/5
        [HttpGet]
        public async Task<IActionResult> EditSchool(int id)
        {
            var username = User.Identity?.Name;
            var admin = await _context.Admins.Include(a => a.User).FirstOrDefaultAsync(a => a.User!.Username == username);
            if (admin == null)
            {
                return Unauthorized();
            }

            var school = await _context.Schools
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id && s.AdminId == admin.Id);

            if (school == null)
            {
                TempData["ErrorMessage"] = "School not found or you do not have permission to edit.";
                return RedirectToAction("SchoolList");
            }

            var viewModel = new SchoolListViewModel
            {
                School = school,
                User = school.User ?? new User()
            };

            return View(viewModel);
        }

        // POST: Admin/EditSchool
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSchool(SchoolListViewModel viewModel)
        {
            var username = User.Identity?.Name;
            var admin = await _context.Admins.Include(a => a.User).FirstOrDefaultAsync(a => a.User!.Username == username);
            if (admin == null)
            {
                TempData["ErrorMessage"] = "Unauthorized access.";
                return RedirectToAction("SchoolList");
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Validation error: " + error.ErrorMessage);
                }
                return View(viewModel);
            }

            var school = await _context.Schools
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == viewModel.School.Id && s.AdminId == admin.Id);

            if (school == null)
            {
                TempData["ErrorMessage"] = "School not found or you do not have permission to edit.";
                return RedirectToAction("SchoolList");
            }

            // Update school fields
            school.SchoolName = viewModel.School.SchoolName;
            school.Tole = viewModel.School.Tole;
            school.TelephoneNo = viewModel.School.TelephoneNo;
            school.WardNo = viewModel.School.WardNo;
            school.AccountNo = viewModel.School.AccountNo;
            school.ContactPerson = viewModel.School.ContactPerson;
            school.Email = viewModel.School.Email;
            school.UserPerson = viewModel.School.UserPerson;
            school.EMISCode = viewModel.School.EMISCode;

            // Update user fields
            if (school.User != null)
            {
                school.User.Username = viewModel.User.Username;

                // If password fields are filled, update password
                if (!string.IsNullOrWhiteSpace(viewModel.Password) || !string.IsNullOrWhiteSpace(viewModel.ConfirmPassword))
                {
                    if (viewModel.Password == viewModel.ConfirmPassword)
                    {
                        school.User.Password = viewModel.Password;
                    }
                    else
                    {
                        ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                        return View(viewModel);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "School updated successfully!";
                return RedirectToAction("SchoolList");
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Unable to save changes. Try again later.";
                Console.WriteLine("Error updating school: " + ex.Message);
                return View(viewModel);
            }
        }

        // POST: Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var username = User.Identity?.Name;
            var admin = await _context.Admins.Include(a => a.User).FirstOrDefaultAsync(a => a.User!.Username == username);
            if (admin == null)
            {
                TempData["ErrorMessage"] = "Unauthorized access.";
                return RedirectToAction("SchoolList");
            }

            var school = await _context.Schools.FirstOrDefaultAsync(s => s.Id == id && s.AdminId == admin.Id);
            if (school == null)
            {
                TempData["ErrorMessage"] = "School not found or you do not have permission to delete.";
                return RedirectToAction("SchoolList");
            }

            _context.Schools.Remove(school);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "School deleted successfully!";
            return RedirectToAction(nameof(SchoolList));
        }
    }
}
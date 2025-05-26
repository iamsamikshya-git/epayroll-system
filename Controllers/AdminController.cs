using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_PayRoll.Data;
using E_PayRoll.Models;
using System.Linq;
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
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult SchoolList()
        {
            var schools = _context.Schools.ToList();
            return View(schools);
        }

        public IActionResult CreateSchool()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateSchool(string name, string address)
        {
            if (_context.Schools.Any(s => s.Name == name))
            {
                ViewBag.Error = "School already exists";
                return View();
            }

            var school = new School { Name = name, Address = address };
            _context.Schools.Add(school);
            _context.SaveChanges();

            return RedirectToAction("SchoolList");
        }

        [HttpPost]
        public IActionResult DeleteSchool(int id)
        {
            var school = _context.Schools.Find(id);
            if (school != null)
            {
                _context.Schools.Remove(school);
                _context.SaveChanges();
            }
            return RedirectToAction("SchoolList");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}

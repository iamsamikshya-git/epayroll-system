using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_PayRoll.Data;
using E_PayRoll.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace E_PayRoll.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SuperAdminController(ApplicationDbContext context) => _context = context;

        // Login GET
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role == "SuperAdmin")
                    return RedirectToAction("Dashboard");
                if (role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }

        // Login POST
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            // NOTE: Use hashed passwords in production!
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

            if (user != null && user.Role == "SuperAdmin")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        // SuperAdmin Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // Admin Creation Form
        public IActionResult CreateAdmin()
        {
            return View();
        }

        // Admin Creation POST
        [HttpPost]
        public IActionResult CreateAdmin(string username, string password)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Username already exists";
                return View();
            }

            // NOTE: Use password hashing here in real-world apps!
            var admin = new User
            {
                Username = username,
                Password = password,
                Role = "Admin"
            };

            _context.Users.Add(admin);
            _context.SaveChanges();

            ViewBag.Message = "Admin created successfully";
            return View();
        }

        // Optional: Logout for SuperAdmin
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}

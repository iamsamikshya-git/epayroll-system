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
            int adminCount = _context.Users.Count(u => u.Role == "Admin");
            ViewBag.AdminCount = adminCount;
            return View();
        }

        // Admin Creation Form
        public IActionResult CreateAdmin()
        {
            return View();
        }

        // Admin Creation POST (for your new form)
        [HttpPost]
        public async Task<IActionResult> Add(
            string username,
            string password,
            string country,
            string province,
            string localBodyName,
            string localBodyType,
            string district,
            string email,
            IFormFile logo)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Username already exists";
                return View("CreateAdmin");
            }

            string? logoPath = null;
            if (logo != null && logo.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(logo.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await logo.CopyToAsync(stream);
                }
                logoPath = "/uploads/" + fileName;
            }

            var admin = new User
            {
                Username = username,
                Password = password,
                Role = "Admin",
                Country = country,
                Province = province,
                LocalBodyName = localBodyName,
                LocalBodyType = localBodyType,
                District = district,
                Email = email,
                LogoPath = logoPath
            };

            _context.Users.Add(admin);
            _context.SaveChanges();

            ViewBag.Message = "Admin created successfully";
            return View("CreateAdmin");
        }


// In SuperAdminController.cs
public IActionResult AdminList()
{
    var admins = _context.Users.Where(u => u.Role == "Admin").ToList();
    return View(admins);
}
        // Optional: Logout for SuperAdmin
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
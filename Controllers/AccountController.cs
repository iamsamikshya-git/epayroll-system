using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_PayRoll.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    public AccountController(ApplicationDbContext context) => _context = context;

    [AllowAnonymous]
    public IActionResult Login()
    {
        // Do NOT redirect here! Always show login form for unauthenticated users.
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Redirect to dashboard based on role
            if (user.Role == "SuperAdmin")
                return RedirectToAction("Dashboard", "SuperAdmin");
            if (user.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");
            if (user.Role == "School")
                return RedirectToAction("Dashboard", "School");
            if (user.Role == "Teacher")
                return RedirectToAction("Dashboard", "Teacher");
        }
        ViewBag.Error = "Invalid credentials";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
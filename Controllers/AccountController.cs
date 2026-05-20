using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_PayRoll.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Role, user.Role!)
                
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
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var username = User?.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized();

        // Get the login user row
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return NotFound("User not found.");

        // SuperAdmin -> (you can render a simple self profile or a dashboard)
        if (User.IsInRole("SuperAdmin"))
        {
            // If you have a SuperAdmin entity, load and show it.
            // Else, send to a simple page or dashboard.
            return RedirectToAction("Dashboard", "SuperAdmin");
        }

        // Admin -> redirect to AdminProfile (you already built this in SuperAdminController)
       if (User.IsInRole("Admin"))
{
    // user == the row we already fetched earlier using username
    var adminId = await _context.Admins
        .Where(a => a.UserId == user.Id)
        .Select(a => a.Id)
        .FirstOrDefaultAsync();

    if (adminId == 0)
        return NotFound("Admin profile not found.");

    return RedirectToAction("AdminProfile", "SuperAdmin", new { id = adminId });
}


        // School -> redirect to SchoolProfile (you already built this in AdminController)
        if (User.IsInRole("School"))
        {
            var schoolId = await _context.Schools
                .Where(s => s.UserId == user.Id)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (schoolId == 0) return NotFound("School profile not found.");

            return RedirectToAction("SchoolProfile", "Admin", new { id = schoolId });
        }

        // Teacher (only if you have teacher login + profile)
        if (User.IsInRole("Teacher"))
        {
            var teacherId = await _context.Teachers
                .Where(t => /* t.UserId == user.Id */ false) // add UserId if your Teacher has one
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            if (teacherId == 0) return NotFound("Teacher profile not found.");

            return RedirectToAction("TeacherProfile", "School", new { id = teacherId });
        }

        // Fallback
        return Forbid();
    }
    [AllowAnonymous]
[HttpGet]
public IActionResult AccessDenied(string? returnUrl = null)
{
    return Content("Access denied.");
}

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
public IActionResult ChangeLanguage(string culture, string returnUrl = " ")
{
    if (!string.IsNullOrEmpty(culture))
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
    }

    if (returnUrl != null && Url.IsLocalUrl(returnUrl))
    {
        return LocalRedirect(returnUrl);
    }

    return RedirectToAction("Index", "Home");
}

}
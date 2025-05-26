using Microsoft.AspNetCore.Mvc;
using E_PayRoll.Data;
using System.Linq;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    public AccountController(ApplicationDbContext context) => _context = context;

    public IActionResult Login()
    {
        // If already logged in, redirect to the correct dashboard
        if (TempData["SuperAdmin"] != null)
            return RedirectToAction("Dashboard", "SuperAdmin");
        if (TempData["Admin"] != null)
            return RedirectToAction("Dashboard", "Admin");
        if (TempData["School"] != null)
            return RedirectToAction("Dashboard", "School");
        if (TempData["Teacher"] != null)
            return RedirectToAction("Dashboard", "Teacher");

        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user != null)
        {
            if (user.Role == "SuperAdmin")
            {
                TempData["SuperAdmin"] = user.Username;
                return RedirectToAction("Dashboard", "SuperAdmin");
            }
            else if (user.Role == "Admin")
            {
                TempData["Admin"] = user.Username;
                return RedirectToAction("Dashboard", "Admin");
            }
            else if (user.Role == "School")
            {
                TempData["School"] = user.Username;
                return RedirectToAction("Dashboard", "School");
            }
            else if (user.Role == "Teacher")
            {
                TempData["Teacher"] = user.Username;
                return RedirectToAction("Dashboard", "Teacher");
            }
        }
        ViewBag.Error = "Invalid credentials";
        return View();
    }

    public IActionResult Logout()
    {
        TempData.Clear();
        return RedirectToAction("Login");
    }
}
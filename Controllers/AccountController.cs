using Microsoft.AspNetCore.Mvc;
using E_PayRoll.Data;
using System.Linq;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    public AccountController(ApplicationDbContext context) => _context = context;

    public IActionResult Login() => View();

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
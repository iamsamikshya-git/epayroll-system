using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_PayRoll.Data;
using E_PayRoll.Models;
using System.Linq;

namespace E_PayRoll.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    public AdminController(ApplicationDbContext context) => _context = context;

    [AllowAnonymous]
    public IActionResult Login() => View();

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password && u.Role == "Admin");
        if (user != null)
        {
            // Here you should sign in the user using ASP.NET Core Identity or authentication cookie
            // For demonstration, we'll just use TempData (not secure for production)
            TempData["Admin"] = user.Username;
            return RedirectToAction("Dashboard");
        }
        ViewBag.Error = "Invalid credentials";
        return View();
    }

    public IActionResult Dashboard()
    {
        // With [Authorize], this check is not strictly needed, but you can keep it for extra safety
        if (TempData["Admin"] == null) return RedirectToAction("Login");
        TempData.Keep("Admin");
        return View();
    }

    public IActionResult SchoolList()
    {
        if (TempData["Admin"] == null) return RedirectToAction("Login");
        TempData.Keep("Admin");
        var schools = _context.Schools.ToList();
        return View(schools);
    }

    public IActionResult CreateSchool()
    {
        if (TempData["Admin"] == null) return RedirectToAction("Login");
        TempData.Keep("Admin");
        return View();
    }

    [HttpPost]
    public IActionResult CreateSchool(string name, string address)
    {
        if (TempData["Admin"] == null) return RedirectToAction("Login");
        TempData.Keep("Admin");
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
        if (TempData["Admin"] == null) return RedirectToAction("Login");
        TempData.Keep("Admin");
        var school = _context.Schools.Find(id);
        if (school != null)
        {
            _context.Schools.Remove(school);
            _context.SaveChanges();
        }
        return RedirectToAction("SchoolList");
    }
}
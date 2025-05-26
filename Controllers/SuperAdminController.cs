using Microsoft.AspNetCore.Mvc;
using E_PayRoll.Data;
using E_PayRoll.Models;
using System.Linq;

namespace E_PayRoll.Controllers;

public class SuperAdminController : Controller
{
    private readonly ApplicationDbContext _context;
    public SuperAdminController(ApplicationDbContext context) => _context = context;

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password && u.Role == "SuperAdmin");
        if (user != null)
        {
            TempData["SuperAdmin"] = user.Username;
            return RedirectToAction("Dashboard");
        }
        ViewBag.Error = "Invalid credentials";
        return View();
    }

    public IActionResult Dashboard()
    {
        if (TempData["SuperAdmin"] == null) return RedirectToAction("Login");
        TempData.Keep("SuperAdmin");
        return View();
    }

    public IActionResult CreateAdmin()
    {
        if (TempData["SuperAdmin"] == null) return RedirectToAction("Login");
        TempData.Keep("SuperAdmin");
        return View();
    }

    [HttpPost]
    public IActionResult CreateAdmin(string username, string password)
    {
        if (TempData["SuperAdmin"] == null) return RedirectToAction("Login");
        TempData.Keep("SuperAdmin");
        if (_context.Users.Any(u => u.Username == username))
        {
            ViewBag.Error = "Username already exists";
            return View();
        }
        var admin = new User { Username = username, Password = password, Role = "Admin" };
        _context.Users.Add(admin);
        _context.SaveChanges();
        ViewBag.Message = "Admin created successfully";
        return View();
    }
}
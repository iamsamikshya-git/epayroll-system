using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_PayRoll.Models;
using E_PayRoll.ViewModels;
using E_PayRoll.Data;
using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Dashboard
    public IActionResult Dashboard()
{
    ViewBag.SchoolCount = _context.Schools.Count();
    return View();
}

        // GET: Admin/SchoolList
        public async Task<IActionResult> SchoolList()
        {
            var schools = await _context.Schools
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
        public async Task<IActionResult> CreateSchool(SchoolListViewModel viewModel, string ConfirmPassword)
        {
            if (viewModel.User == null || string.IsNullOrEmpty(viewModel.User.Password) || viewModel.User.Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match or are empty.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Create and Save User
            var user = new User
            {
                Username = viewModel.User?.Username ?? viewModel.School.Email,
                Password = viewModel.User?.Password, // Note: Hashing should be applied in production
                Role = viewModel.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign first available Admin
            var admin = await _context.Admins.FirstOrDefaultAsync();
            if (admin == null)
            {
                ModelState.AddModelError("", "No admin available to assign.");
                return View(viewModel);
            }

            // Create School linked with user and admin
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
            var school = await _context.Schools
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (school == null)
            {
                return NotFound();
            }

            var viewModel = new SchoolListViewModel
            {
                School = school,
                User = school.User ?? new User()
            };

            return View(viewModel);
        }

      [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditSchool(SchoolListViewModel viewModel)
{
    if (!ModelState.IsValid)
    {
        return View(viewModel);
    }

    var school = await _context.Schools
        .Include(s => s.User)
        .FirstOrDefaultAsync(s => s.Id == viewModel.School.Id);

    if (school == null)
    {
        return NotFound();
    }

    // Update school properties
    school.SchoolName = viewModel.School.SchoolName;
    school.Tole = viewModel.School.Tole;
    school.TelephoneNo = viewModel.School.TelephoneNo;
    school.WardNo = viewModel.School.WardNo;
    school.AccountNo = viewModel.School.AccountNo;
    school.ContactPerson = viewModel.School.ContactPerson;
    school.Email = viewModel.School.Email;
    school.UserPerson = viewModel.School.UserPerson;
    school.EMISCode = viewModel.School.EMISCode;

    // Update user properties if available
    if (school.User != null)
    {
        school.User.Username = viewModel.User.Username;

        if (!string.IsNullOrWhiteSpace(viewModel.User.Password))
        {
            // No hashing used, save plain text password
            school.User.Password = viewModel.User.Password;
        }

        _context.Entry(school.User).State = EntityState.Modified;
    }

    _context.Entry(school).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "School updated successfully.";
        return RedirectToAction("SchoolList");
    }
    catch (DbUpdateException ex)
    {
        ModelState.AddModelError("", "Unable to save changes. Try again later.");
        Console.WriteLine("Error updating school: " + ex.Message);
    }

    return View(viewModel);
}

        // POST: Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var school = await _context.Schools.FindAsync(id);
            if (school == null)
            {
                return NotFound();
            }

            _context.Schools.Remove(school);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "School deleted successfully!";
            return RedirectToAction(nameof(SchoolList));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_PayRoll.Data;
using E_PayRoll.Models;
using E_PayRoll.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace E_PayRoll.Controllers
{
    [Authorize(Roles = "School")]
    public class SchoolController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SchoolController(ApplicationDbContext context) => _context = context;

        // Helper to get current schoolId from logged-in user
 private async Task<int?> GetCurrentSchoolIdAsync()
{
    var username = User.Identity?.Name;
    if (string.IsNullOrEmpty(username))
        return null;

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    if (user == null)
        return null;

    var school = await _context.Schools.FirstOrDefaultAsync(s => s.UserId == user.Id);
    return school?.Id;
} // Dashboard
        public IActionResult Dashboard()
        {
            ViewBag.TeacherCount = _context.Teachers.Count();
            return View();
        }

        // GET: School/AddTeacher
        [HttpGet]
        public IActionResult AddTeacher()
        {
            return View(new TeacherListViewModel
            {
                Teacher = new Teacher()
            });
        }

        // POST: School/AddTeacher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeacher(TeacherListViewModel viewModel)
        {
            viewModel.Role = "Teacher";

            if (string.IsNullOrEmpty(viewModel.Password) || viewModel.Password != viewModel.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match or are empty.");

            if (viewModel.Teacher == null)
                ModelState.AddModelError("", "Teacher details are missing.");

            if (await _context.Users.AnyAsync(u => u.Username == viewModel.Username))
                ModelState.AddModelError("Username", "Username already taken");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Validation error: " + error.ErrorMessage);
                }
                return View(viewModel);
            }

            var admin = await _context.Admins.FirstOrDefaultAsync();
            if (admin == null)
            {
                ModelState.AddModelError("", "No admin found. Please create an admin first.");
                return View(viewModel);
            }

            // Get current schoolId
            var schoolId = await GetCurrentSchoolIdAsync();
            if (schoolId == null)
            {
                ModelState.AddModelError("", "Unable to determine your school. Please contact admin.");
                return View(viewModel);
            }

            // Create and Save User
            var user = new User
            {
                Username = viewModel.Username,
                Password = viewModel.Password,
                Role = viewModel.Role
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // File upload logic
            string? photoPath = null;
            if (viewModel.PhotoFile != null && viewModel.PhotoFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(viewModel.PhotoFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.PhotoFile.CopyToAsync(stream);
                }
                photoPath = "/uploads/" + fileName;
            }

            string? cvPath = null;
            if (viewModel.CVFile != null && viewModel.CVFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(viewModel.CVFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.CVFile.CopyToAsync(stream);
                }
                cvPath = "/uploads/" + fileName;
            }

            var teacher = new Teacher
            {
                FirstName = viewModel.Teacher!.FirstName,
                MiddleName = viewModel.Teacher.MiddleName,
                LastName = viewModel.Teacher.LastName,
                Gender = viewModel.Teacher.Gender,
                DateOfBirth = viewModel.Teacher.DateOfBirth,
                EmployeeId = viewModel.Teacher.EmployeeId,
                Department = viewModel.Teacher.Department,
                Subjects = viewModel.Teacher.Subjects,
                Designation = viewModel.Teacher.Designation,
                DateOfJoining = viewModel.Teacher.DateOfJoining,
                LanguagePreference = viewModel.Teacher.LanguagePreference,
                AdminId = admin.Id,
                UserId = user.Id,
                SchoolId = schoolId.Value,
                PhotoPath = photoPath,
                CVPath = cvPath
            };

            try
            {
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Teacher created successfully!";
                return RedirectToAction("TeacherList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saving to database: " + ex.Message;
                Console.WriteLine("DB ERROR: " + ex.Message);
                return View(viewModel);
            }
        }

        // GET: School/TeacherList
        [HttpGet]
        public async Task<IActionResult> TeacherList()
        {
            var schoolId = await GetCurrentSchoolIdAsync();
            if (schoolId == null)
                return Unauthorized();

            var teachers = await _context.Teachers
                .Where(t => t.SchoolId == schoolId.Value)
                .Include(t => t.User)
                .ToListAsync();
            return View(teachers);
        }

        // GET: School/EditTeacher/5
        [HttpGet]
        public async Task<IActionResult> EditTeacher(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
                return NotFound();

            var schoolId = await GetCurrentSchoolIdAsync();
            if (schoolId == null || teacher.SchoolId != schoolId.Value)
                return Unauthorized();

            var viewModel = new TeacherListViewModel
            {
                Teacher = teacher,
                Username = teacher.User?.Username ?? "",
                // Do not set Password for security reasons
                // ConfirmPassword left blank
            };

            return View(viewModel);
        }

        // POST: School/EditTeacher/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeacher(int id, TeacherListViewModel viewModel)
        {
            if (id != viewModel.Teacher!.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Validation error: " + error.ErrorMessage);
                }
                return View(viewModel);
            }

            var teacher = await _context.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (teacher == null)
                return NotFound();

            var schoolId = await GetCurrentSchoolIdAsync();
            if (schoolId == null || teacher.SchoolId != schoolId.Value)
                return Unauthorized();

            // File upload logic for PhotoFile and CVFile
            if (viewModel.PhotoFile != null && viewModel.PhotoFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(viewModel.PhotoFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.PhotoFile.CopyToAsync(stream);
                }
                teacher.PhotoPath = "/uploads/" + fileName;
            }

            if (viewModel.CVFile != null && viewModel.CVFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(viewModel.CVFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.CVFile.CopyToAsync(stream);
                }
                teacher.CVPath = "/uploads/" + fileName;
            }

            // Update teacher fields
            teacher.FirstName = viewModel.Teacher.FirstName;
            teacher.MiddleName = viewModel.Teacher.MiddleName;
            teacher.LastName = viewModel.Teacher.LastName;
            teacher.Gender = viewModel.Teacher.Gender;
            teacher.DateOfBirth = viewModel.Teacher.DateOfBirth;
            teacher.EmployeeId = viewModel.Teacher.EmployeeId;
            teacher.Department = viewModel.Teacher.Department;
            teacher.Subjects = viewModel.Teacher.Subjects;
            teacher.Designation = viewModel.Teacher.Designation;
            teacher.DateOfJoining = viewModel.Teacher.DateOfJoining;
            teacher.LanguagePreference = viewModel.Teacher.LanguagePreference;

            // Update the linked User as well
            if (teacher.User != null)
            {
                teacher.User.Username = viewModel.Username;
                if (!string.IsNullOrWhiteSpace(viewModel.Password))
                {
                    if (viewModel.ConfirmPassword != viewModel.Password)
                    {
                        ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                        return View(viewModel);
                    }
                    teacher.User.Password = viewModel.Password;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Teacher updated successfully!";
            return RedirectToAction("TeacherList");
        }

        // POST: School/DeleteTeacher/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Teacher not found.";
                return RedirectToAction("TeacherList");
            }

            var schoolId = await GetCurrentSchoolIdAsync();
            if (schoolId == null || teacher.SchoolId != schoolId.Value)
            {
                TempData["ErrorMessage"] = "Unauthorized delete attempt.";
                return RedirectToAction("TeacherList");
            }

            // Also delete the linked user
            var user = await _context.Users.FindAsync(teacher.UserId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Teacher and user deleted successfully!";
            return RedirectToAction("TeacherList");
        }

        // GET: School/Salary
        [HttpGet]
        public async Task<IActionResult> Salary()
        {
            var schoolId = await GetCurrentSchoolIdAsync();
            if (schoolId == null)
                return Unauthorized();

            var teachers = await _context.Teachers
                .Where(t => t.SchoolId == schoolId.Value)
                .ToListAsync();

            return View(teachers);
        }
    }
}
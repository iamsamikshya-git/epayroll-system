// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using E_PayRoll.Data;
// using Microsoft.EntityFrameworkCore;
// using System.Threading.Tasks;
// using System.Linq;

// namespace E_PayRoll.Controllers
// {
//     [Authorize(Roles = "Teacher")]
//     public class TeacherController : Controller
//     {
//         private readonly ApplicationDbContext _context;

//         public TeacherController(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         // GET: Teacher/Dashboard
//         public async Task<IActionResult> Dashboard()
//         {
//             var username = User.Identity?.Name;
//             if (string.IsNullOrEmpty(username))
//                 return Unauthorized();

//             // Find the user
//             var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
//             if (user == null)
//                 return Unauthorized();

//             // Find the teacher details for this user
//             var teacher = await _context.Teachers
//                 .Include(t => t.School)
//                 .Include(t => t.Admin)
//                 .FirstOrDefaultAsync(t => t.UserId == user.Id);

//             if (teacher == null)
//                 return NotFound("Teacher details not found.");

//             return View(teacher);
//         }
//     }
// }
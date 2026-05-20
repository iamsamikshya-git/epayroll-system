// Controllers/NotificationsController.cs
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_PayRoll.Data;

namespace E_PayRoll.Controllers
{
    [Authorize]
    [Route("notifications")]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NotificationsController(ApplicationDbContext ctx) => _context = ctx;

        [HttpGet("latest")]
        public async Task<IActionResult> Latest(int take = 10)
        {
            var username = User?.Identity?.Name ?? string.Empty;

            var items = await _context.Notifications
                .Where(n => n.RecipientUsername == username)
                .OrderByDescending(n => n.CreatedAtUtc)
                .Take(take)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.LinkUrl,
                    n.IsRead,
                    n.CreatedAtUtc
                })
                .ToListAsync();

            var unread = items.Count(i => !i.IsRead);
            return Json(new { unread, items });
        }

        [HttpPost("read/{id:int}")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var username = User?.Identity?.Name ?? string.Empty;

            var n = await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == id && x.RecipientUsername == username);

            if (n == null) return NotFound();

            n.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            var username = User?.Identity?.Name ?? string.Empty;

            var items = await _context.Notifications
                .Where(x => x.RecipientUsername == username && !x.IsRead)
                .ToListAsync();

            foreach (var n in items) n.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using E_PayRoll.Data;
using E_PayRoll.Hubs;
using E_PayRoll.Models;

namespace E_PayRoll.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(ApplicationDbContext db, IHubContext<NotificationHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        public async Task CreateAsync(string recipientUsername, string title, string message, string? linkUrl, NotificationType type)
        {
            var n = new Notification
            {
                RecipientUsername = recipientUsername,
                Title = title,
                Message = message,
                LinkUrl = linkUrl,
                Type = type,
                IsRead = false,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.Notifications.Add(n);
            await _db.SaveChangesAsync();

            var payload = new {
                id = n.Id, title = n.Title, message = n.Message,
                linkUrl = n.LinkUrl, isRead = n.IsRead, createdAtUtc = n.CreatedAtUtc
            };

            // 1) Username group
            await _hub.Clients.Group($"user-{recipientUsername}").SendAsync("NotifyNew", payload);

            // 2) UserId group (look it up from Users table)
            var uid = await _db.Users
                .Where(u => u.Username == recipientUsername)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync();

            if (uid.HasValue)
                await _hub.Clients.Group($"uid-{uid.Value}").SendAsync("NotifyNew", payload);
        }

        public Task<int> CountUnreadAsync(string recipientUsername) =>
            _db.Notifications.CountAsync(x => x.RecipientUsername == recipientUsername && !x.IsRead);
    }
}

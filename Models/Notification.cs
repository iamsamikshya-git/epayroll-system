// Models/Notification.cs
namespace E_PayRoll.Models
{
    public enum NotificationType { General = 0, SalarySubmitted = 1, SalaryApproved = 2 }

    public class Notification
    {
        public int Id { get; set; }
        public string RecipientUsername { get; set; } = ""; // we’ll group SignalR by username
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? LinkUrl { get; set; }
        public bool IsRead { get; set; } = false;
        public NotificationType Type { get; set; } = NotificationType.General;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

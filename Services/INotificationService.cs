// Services/INotificationService.cs
using System.Threading.Tasks;
using E_PayRoll.Models;

namespace E_PayRoll.Services
{
    public interface INotificationService
    {
        Task CreateAsync(
            string recipientUsername,
            string title,
            string message,
            string? linkUrl,
            NotificationType type);

        Task<int> CountUnreadAsync(string recipientUsername);
    }
}

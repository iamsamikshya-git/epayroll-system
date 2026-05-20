using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace E_PayRoll.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
            var userId   = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrWhiteSpace(username))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{username}");

            if (!string.IsNullOrWhiteSpace(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"uid-{userId}");

            await base.OnConnectedAsync();
        }
    }
}

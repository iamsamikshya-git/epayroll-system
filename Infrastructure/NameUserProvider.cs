// /Infrastructure/NameUserIdProvider.cs
using Microsoft.AspNetCore.SignalR;

namespace E_PayRoll.Infrastructure
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
            => connection.User?.Identity?.Name; // <— username
    }
}

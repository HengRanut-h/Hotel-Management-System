using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HotelManagement.Api.Hubs;

[Authorize]
public sealed class NotificationHub : Hub
{
}

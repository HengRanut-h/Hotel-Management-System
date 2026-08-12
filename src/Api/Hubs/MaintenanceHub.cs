using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HotelManagement.Api.Hubs;

[Authorize]
public sealed class MaintenanceHub : Hub
{
    public Task JoinHotel(string hotelId) => Groups.AddToGroupAsync(Context.ConnectionId, $"hotel:{hotelId}");
}
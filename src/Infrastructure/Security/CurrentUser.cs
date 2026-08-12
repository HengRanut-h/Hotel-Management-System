using System.Security.Claims;
using HotelManagement.Application.Abstractions.Security;

namespace HotelManagement.Infrastructure.Security;

public sealed class CurrentUser(
    IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal =>
        accessor.HttpContext?.User;

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated == true;

    public Guid? UserId =>
        ParseGuid(
            Principal?.FindFirstValue(
                ClaimTypes.NameIdentifier));

    public Guid? HotelId =>
        ParseGuid(
            Principal?.FindFirstValue("hotel_id"));

    public Guid? BranchId =>
        ParseGuid(
            Principal?.FindFirstValue("branch_id"));

    public string? Email =>
        Principal?.FindFirstValue(
            ClaimTypes.Email);

    private static Guid? ParseGuid(string? value) =>
        Guid.TryParse(value, out var id) ? id : null;
}

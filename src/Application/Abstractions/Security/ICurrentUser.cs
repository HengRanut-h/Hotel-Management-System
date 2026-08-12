namespace HotelManagement.Application.Abstractions.Security;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    Guid? HotelId { get; }
    Guid? BranchId { get; }
    string? Email { get; }
}

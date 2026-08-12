using HotelManagement.Domain.Common.Entities;
using HotelManagement.Domain.Modules.Hotels.Entities;

namespace HotelManagement.Domain.Modules.Guests.Entities;

public sealed class Guest : AuditableEntity
{
    private Guest() { }

    public Guest(Guid hotelId, string firstName, string lastName, string? phone, string? email)
    {
        HotelId = hotelId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Phone = phone?.Trim();
        Email = email?.Trim().ToLowerInvariant();
    }

    public Guid HotelId { get; private set; }
    public Hotel Hotel { get; private set; } = null!;
    public Guid? UserId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public bool IsVip { get; private set; }

    public void Update(string firstName, string lastName, string? phone, string? email)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Phone = phone?.Trim();
        Email = email?.Trim().ToLowerInvariant();
    }
}

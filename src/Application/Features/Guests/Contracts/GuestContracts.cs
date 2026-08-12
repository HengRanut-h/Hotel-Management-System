using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Application.Features.Guests.Contracts;

public sealed class CreateGuestRequest
{
    [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [MaxLength(50)] public string? Phone { get; set; }
    [EmailAddress, MaxLength(200)] public string? Email { get; set; }
}

public sealed class UpdateGuestRequest
{
    [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [MaxLength(50)] public string? Phone { get; set; }
    [EmailAddress, MaxLength(200)] public string? Email { get; set; }
}

public sealed class GuestResponse
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsVip { get; set; }
}

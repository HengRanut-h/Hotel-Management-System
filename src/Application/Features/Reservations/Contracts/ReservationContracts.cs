using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Application.Features.Reservations.Contracts;

public sealed class CreateReservationRequest
{
    public Guid? BranchId { get; set; }
    public Guid GuestId { get; set; }
    public Guid RoomTypeId { get; set; }
    public Guid? RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    [Range(1, 20)] public int Adults { get; set; } = 1;
    [Range(0, 20)] public int Children { get; set; }
    [Range(0.01, 999999)] public decimal NightlyRate { get; set; }
}

public sealed class CheckInRequest
{
    public Guid RoomId { get; set; }
}

public sealed class ReservationResponse
{
    public Guid Id { get; set; }
    public string ReservationNumber { get; set; } = string.Empty;
    public Guid GuestId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public Guid RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public Guid? RoomId { get; set; }
    public string? RoomNumber { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Nights { get; set; }
    public decimal NightlyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

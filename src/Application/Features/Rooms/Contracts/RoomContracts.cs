using System.ComponentModel.DataAnnotations;
using HotelManagement.Domain.Modules.Rooms.Enums;

namespace HotelManagement.Application.Features.Rooms.Contracts;

public sealed class CreateRoomRequest
{
    public Guid? BranchId { get; set; }
    public Guid RoomTypeId { get; set; }
    [Required, MaxLength(20)] public string RoomNumber { get; set; } = string.Empty;
    [Range(0, 1000)] public int Floor { get; set; }
}

public sealed class UpdateRoomRequest
{
    public Guid RoomTypeId { get; set; }
    [Required, MaxLength(20)] public string RoomNumber { get; set; } = string.Empty;
    [Range(0, 1000)] public int Floor { get; set; }
}

public sealed class ChangeRoomStatusRequest
{
    public RoomStatus Status { get; set; }
}

public sealed class RoomResponse
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public Guid BranchId { get; set; }
    public Guid RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public string Status { get; set; } = string.Empty;
}

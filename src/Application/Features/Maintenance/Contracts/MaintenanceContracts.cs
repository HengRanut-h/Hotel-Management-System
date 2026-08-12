using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Application.Features.Maintenance.Contracts;

public sealed class CreateMaintenanceRequest
{
    public Guid? RoomId { get; set; }

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Priority { get; set; } = "Normal";
}

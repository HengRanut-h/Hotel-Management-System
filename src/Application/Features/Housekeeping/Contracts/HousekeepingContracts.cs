using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Application.Features.Housekeeping.Contracts;

public sealed class CreateHousekeepingTaskRequest
{
    public Guid RoomId { get; set; }

    [Required, MaxLength(100)]
    public string TaskType { get; set; } = "Checkout Cleaning";

    [Required, MaxLength(30)]
    public string Priority { get; set; } = "Normal";
}

using System.ComponentModel.DataAnnotations;
using HotelManagement.Domain.Modules.Utilities.Enums;

namespace HotelManagement.Application.Features.Utilities.Contracts;

public sealed class CreateUtilityMeterRequest
{
    public Guid RoomId { get; set; }
    public UtilityType Type { get; set; }
    [Required, MaxLength(100)] public string MeterNumber { get; set; } = string.Empty;
    [Range(0, 999999)] public decimal RatePerUnit { get; set; }
}

public sealed class RecordMeterReadingRequest
{
    public Guid UtilityMeterId { get; set; }
    [Range(0, 999999999)] public decimal CurrentReading { get; set; }
    public DateOnly ReadingDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}

public sealed class UtilityMeterResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string MeterNumber { get; set; } = string.Empty;
    public decimal RatePerUnit { get; set; }
    public decimal LastReading { get; set; }
}

public sealed class MeterReadingResponse
{
    public Guid Id { get; set; }
    public Guid UtilityMeterId { get; set; }
    public decimal PreviousReading { get; set; }
    public decimal CurrentReading { get; set; }
    public decimal Usage { get; set; }
    public decimal RatePerUnit { get; set; }
    public decimal Amount { get; set; }
    public DateOnly ReadingDate { get; set; }
}

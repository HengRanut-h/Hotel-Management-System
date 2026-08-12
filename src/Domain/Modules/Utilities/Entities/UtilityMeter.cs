using HotelManagement.Domain.Common.Entities;
using HotelManagement.Domain.Modules.Hotels.Entities;
using HotelManagement.Domain.Modules.Rooms.Entities;
using HotelManagement.Domain.Modules.Utilities.Enums;

namespace HotelManagement.Domain.Modules.Utilities.Entities;

public sealed class UtilityMeter : AuditableEntity
{
    private UtilityMeter() { }

    public UtilityMeter(Guid hotelId, Guid roomId, UtilityType type, string meterNumber, decimal ratePerUnit)
    {
        HotelId = hotelId;
        RoomId = roomId;
        Type = type;
        MeterNumber = meterNumber.Trim();
        RatePerUnit = ratePerUnit;
    }

    public Guid HotelId { get; private set; }
    public Hotel Hotel { get; private set; } = null!;
    public Guid RoomId { get; private set; }
    public Room Room { get; private set; } = null!;
    public UtilityType Type { get; private set; }
    public string MeterNumber { get; private set; } = string.Empty;
    public decimal RatePerUnit { get; private set; }
    public decimal LastReading { get; private set; }

    public void SetRate(decimal ratePerUnit) => RatePerUnit = ratePerUnit;
    public void SetLastReading(decimal reading) => LastReading = reading;
}

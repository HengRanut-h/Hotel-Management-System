using HotelManagement.Domain.Common.Entities;

namespace HotelManagement.Domain.Modules.Utilities.Entities;

public sealed class MeterReading : AuditableEntity
{
    private MeterReading() { }

    public MeterReading(Guid utilityMeterId, decimal previousReading, decimal currentReading, decimal ratePerUnit, DateOnly readingDate)
    {
        if (currentReading < previousReading)
            throw new ArgumentException("Current reading cannot be less than previous reading.");

        UtilityMeterId = utilityMeterId;
        PreviousReading = previousReading;
        CurrentReading = currentReading;
        Usage = currentReading - previousReading;
        RatePerUnit = ratePerUnit;
        Amount = Usage * RatePerUnit;
        ReadingDate = readingDate;
    }

    public Guid UtilityMeterId { get; private set; }
    public UtilityMeter UtilityMeter { get; private set; } = null!;
    public decimal PreviousReading { get; private set; }
    public decimal CurrentReading { get; private set; }
    public decimal Usage { get; private set; }
    public decimal RatePerUnit { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly ReadingDate { get; private set; }
}

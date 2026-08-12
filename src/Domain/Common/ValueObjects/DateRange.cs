namespace HotelManagement.Domain.Common.ValueObjects;

public readonly record struct DateRange(DateOnly Start, DateOnly End)
{
    public int Days => End.DayNumber - Start.DayNumber;
    public bool IsValid => End > Start;
    public bool Overlaps(DateRange other) => Start < other.End && End > other.Start;
}
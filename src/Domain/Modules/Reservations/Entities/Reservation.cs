using HotelManagement.Domain.Common.Entities;
using HotelManagement.Domain.Common.Exceptions;
using HotelManagement.Domain.Modules.Guests.Entities;
using HotelManagement.Domain.Modules.Hotels.Entities;
using HotelManagement.Domain.Modules.Reservations.Enums;
using HotelManagement.Domain.Modules.Rooms.Entities;

namespace HotelManagement.Domain.Modules.Reservations.Entities;

public sealed class Reservation : AuditableEntity
{
    private Reservation() { }

    public Reservation(
        Guid hotelId,
        Guid branchId,
        string reservationNumber,
        Guid guestId,
        Guid roomTypeId,
        Guid? roomId,
        DateOnly checkInDate,
        DateOnly checkOutDate,
        int adults,
        int children,
        decimal nightlyRate)
    {
        if (checkOutDate <= checkInDate)
            throw new DomainException("Check-out date must be after check-in date.");

        HotelId = hotelId;
        BranchId = branchId;
        ReservationNumber = reservationNumber;
        GuestId = guestId;
        RoomTypeId = roomTypeId;
        RoomId = roomId;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        Adults = adults;
        Children = children;
        NightlyRate = nightlyRate;
        TotalAmount = nightlyRate * Nights;
        Status = ReservationStatus.Confirmed;
    }

    public Guid HotelId { get; private set; }
    public Hotel Hotel { get; private set; } = null!;
    public Guid BranchId { get; private set; }
    public HotelBranch Branch { get; private set; } = null!;
    public string ReservationNumber { get; private set; } = string.Empty;
    public Guid GuestId { get; private set; }
    public Guest Guest { get; private set; } = null!;
    public Guid RoomTypeId { get; private set; }
    public RoomType RoomType { get; private set; } = null!;
    public Guid? RoomId { get; private set; }
    public Room? Room { get; private set; }
    public DateOnly CheckInDate { get; private set; }
    public DateOnly CheckOutDate { get; private set; }
    public int Adults { get; private set; }
    public int Children { get; private set; }
    public decimal NightlyRate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public ReservationStatus Status { get; private set; }
    public int Nights => CheckOutDate.DayNumber - CheckInDate.DayNumber;

    public void Cancel()
    {
        if (Status is ReservationStatus.CheckedOut or ReservationStatus.Cancelled)
            throw new DomainException("Reservation cannot be cancelled in its current status.");
        Status = ReservationStatus.Cancelled;
    }


    public void MarkNoShow()
    {
        if (Status != ReservationStatus.Confirmed)
            throw new DomainException("Only confirmed reservations can be marked no-show.");
        Status = ReservationStatus.NoShow;
    }

    public void CheckIn(Guid roomId)
    {
        if (Status != ReservationStatus.Confirmed)
            throw new DomainException("Only confirmed reservations can check in.");
        RoomId = roomId;
        Status = ReservationStatus.CheckedIn;
    }

    public void CheckOut()
    {
        if (Status != ReservationStatus.CheckedIn)
            throw new DomainException("Only checked-in reservations can check out.");
        Status = ReservationStatus.CheckedOut;
    }
}

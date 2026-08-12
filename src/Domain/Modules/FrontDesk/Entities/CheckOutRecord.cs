using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.FrontDesk.Entities;
public sealed class CheckOutRecord : AuditableEntity
{
    private CheckOutRecord() { }
    public CheckOutRecord(Guid hotelId, Guid branchId, Guid reservationId, Guid? roomId, Guid guestId, DateTimeOffset checkedOutAtUtc, Guid? performedBy)
    { HotelId=hotelId; BranchId=branchId; ReservationId=reservationId; RoomId=roomId; GuestId=guestId; CheckedOutAtUtc=checkedOutAtUtc; PerformedBy=performedBy; }
    public Guid HotelId {get;private set;} public Guid BranchId {get;private set;} public Guid ReservationId {get;private set;}
    public Guid? RoomId {get;private set;} public Guid GuestId {get;private set;} public DateTimeOffset CheckedOutAtUtc {get;private set;} public Guid? PerformedBy {get;private set;}
}

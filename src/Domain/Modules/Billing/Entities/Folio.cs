using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Billing.Entities;
public sealed class Folio : AuditableEntity
{
    private Folio() { }
    public Folio(Guid hotelId, Guid branchId, Guid reservationId, Guid guestId, string number)
    { HotelId=hotelId; BranchId=branchId; ReservationId=reservationId; GuestId=guestId; FolioNumber=number; }
    public Guid HotelId {get;private set;} public Guid BranchId {get;private set;} public Guid ReservationId {get;private set;} public Guid GuestId {get;private set;}
    public string FolioNumber {get;private set;}=string.Empty; public bool IsClosed {get;private set;} public ICollection<FolioCharge> Charges {get;private set;}=[];
    public decimal Total => Charges.Where(x=>!x.IsVoided).Sum(x=>x.Amount);
    public void Close() { IsClosed=true; MarkUpdated(); }
}

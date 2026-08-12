using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Restaurant.Entities;
public sealed class RestaurantOrder : AuditableEntity
{
    private RestaurantOrder(){} public RestaurantOrder(Guid hotelId,Guid branchId,string number,Guid? roomId,Guid? guestId){HotelId=hotelId;BranchId=branchId;OrderNumber=number;RoomId=roomId;GuestId=guestId;}
    public Guid HotelId{get;private set;} public Guid BranchId{get;private set;} public string OrderNumber{get;private set;}=string.Empty; public Guid? RoomId{get;private set;} public Guid? GuestId{get;private set;}
    public string Status{get;private set;}="Open"; public ICollection<RestaurantOrderItem> Items{get;private set;}=[]; public decimal Total=>Items.Sum(x=>x.Quantity*x.UnitPrice);
    public void ChangeStatus(string status){Status=status.Trim();MarkUpdated();}
}

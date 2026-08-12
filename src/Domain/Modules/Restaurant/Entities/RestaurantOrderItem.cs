using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Restaurant.Entities;
public sealed class RestaurantOrderItem : AuditableEntity
{
    private RestaurantOrderItem(){} public RestaurantOrderItem(Guid orderId,Guid menuItemId,int quantity,decimal unitPrice){OrderId=orderId;MenuItemId=menuItemId;Quantity=quantity;UnitPrice=unitPrice;}
    public Guid OrderId{get;private set;} public Guid MenuItemId{get;private set;} public int Quantity{get;private set;} public decimal UnitPrice{get;private set;}
}

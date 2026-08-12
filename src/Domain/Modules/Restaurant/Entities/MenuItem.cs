using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Restaurant.Entities;
public sealed class MenuItem : AuditableEntity
{
    private MenuItem(){} public MenuItem(Guid hotelId,Guid categoryId,string name,decimal price){HotelId=hotelId;CategoryId=categoryId;Name=name.Trim();Price=price;}
    public Guid HotelId{get;private set;} public Guid CategoryId{get;private set;} public string Name{get;private set;}=string.Empty; public decimal Price{get;private set;} public bool IsAvailable{get;private set;}=true;
}

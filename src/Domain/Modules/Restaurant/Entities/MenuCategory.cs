using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Restaurant.Entities;
public sealed class MenuCategory : AuditableEntity
{
    private MenuCategory(){} public MenuCategory(Guid hotelId,string name){HotelId=hotelId;Name=name.Trim();}
    public Guid HotelId{get;private set;} public string Name{get;private set;}=string.Empty; public bool IsActive{get;private set;}=true;
}

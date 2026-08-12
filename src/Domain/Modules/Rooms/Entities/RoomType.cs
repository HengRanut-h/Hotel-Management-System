using HotelManagement.Domain.Common.Entities; using HotelManagement.Domain.Common.Interfaces; using HotelManagement.Domain.Modules.Hotels.Entities;
namespace HotelManagement.Domain.Modules.Rooms.Entities;
public sealed class RoomType : AuditableEntity, ICatalogEntity
{
 private RoomType(){} public RoomType(Guid hotelId,string name,string code,decimal baseRate,int maxAdults,int maxChildren,string? description=null){HotelId=hotelId;Name=name.Trim();Code=code.Trim().ToUpperInvariant();BaseRate=baseRate;MaxAdults=maxAdults;MaxChildren=maxChildren;Description=description?.Trim();}
 public Guid HotelId{get;private set;} public Hotel Hotel{get;private set;}=null!; public Guid? BranchId=>null; public string Name{get;private set;}=string.Empty; public string Code{get;private set;}=string.Empty; public string? Description{get;private set;} public bool IsActive{get;private set;}=true;
 public decimal BaseRate{get;private set;} public int MaxAdults{get;private set;} public int MaxChildren{get;private set;}
 public void Update(string name,string code,decimal baseRate,int maxAdults,int maxChildren,string? description){Name=name.Trim();Code=code.Trim().ToUpperInvariant();BaseRate=baseRate;MaxAdults=maxAdults;MaxChildren=maxChildren;Description=description?.Trim();MarkUpdated();}
 public void UpdateCatalog(string name,string code,string? description){Name=name.Trim();Code=code.Trim().ToUpperInvariant();Description=description?.Trim();MarkUpdated();} public void SetActive(bool active){IsActive=active;MarkUpdated();}
}

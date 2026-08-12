using HotelManagement.Domain.Common.Entities; using HotelManagement.Domain.Common.Interfaces;
namespace HotelManagement.Domain.Modules.Hotels.Entities;
public sealed class HotelBranch : AuditableEntity, ICatalogEntity
{
 private HotelBranch(){} public HotelBranch(Guid hotelId,string name,string code,string? description=null){HotelId=hotelId;Name=name.Trim();Code=code.Trim().ToUpperInvariant();Description=description?.Trim();}
 public Guid HotelId{get;private set;} public Hotel Hotel{get;private set;}=null!; public Guid? BranchId=>Id; public string Name{get;private set;}=string.Empty; public string Code{get;private set;}=string.Empty; public string? Description{get;private set;} public bool IsActive{get;private set;}=true;
 public void UpdateCatalog(string name,string code,string? description){Name=name.Trim();Code=code.Trim().ToUpperInvariant();Description=description?.Trim();MarkUpdated();} public void SetActive(bool active){IsActive=active;MarkUpdated();}
}

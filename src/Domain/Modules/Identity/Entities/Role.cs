using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Identity.Entities;
public sealed class Role : AuditableEntity
{
 private Role(){} public Role(string name){Update(name);} public string Name{get;private set;}=string.Empty; public string NormalizedName{get;private set;}=string.Empty;
 public ICollection<UserRole>UserRoles{get;private set;}=new List<UserRole>(); public ICollection<RolePermission>RolePermissions{get;private set;}=new List<RolePermission>();
 public void Update(string name){Name=name.Trim();NormalizedName=Name.ToUpperInvariant();MarkUpdated();}
}

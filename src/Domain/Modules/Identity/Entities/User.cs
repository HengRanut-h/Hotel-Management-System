using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Identity.Entities;
public sealed class User : AuditableEntity
{
 private User(){} public User(string fullName,string email){UpdateProfile(fullName,email);} public string FullName{get;private set;}=string.Empty; public string Email{get;private set;}=string.Empty; public string PasswordHash{get;private set;}=string.Empty; public bool IsActive{get;private set;}=true; public Guid? HotelId{get;private set;} public Guid? BranchId{get;private set;}
 public ICollection<UserRole>UserRoles{get;private set;}=new List<UserRole>(); public ICollection<RefreshToken>RefreshTokens{get;private set;}=new List<RefreshToken>();
 public void SetPasswordHash(string hash){PasswordHash=hash;MarkUpdated();} public void SetScope(Guid? hotelId,Guid? branchId){HotelId=hotelId;BranchId=branchId;MarkUpdated();} public void UpdateProfile(string fullName,string email){FullName=fullName.Trim();Email=email.Trim().ToLowerInvariant();MarkUpdated();} public void Disable(){IsActive=false;MarkUpdated();} public void Enable(){IsActive=true;MarkUpdated();}
}

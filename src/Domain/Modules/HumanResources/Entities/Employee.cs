using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.HumanResources.Entities;
public sealed class Employee : AuditableEntity
{
    private Employee(){} public Employee(Guid hotelId,Guid? branchId,string employeeNo,string fullName,string? email,Guid? departmentId,Guid? positionId)
    {HotelId=hotelId;BranchId=branchId;EmployeeNumber=employeeNo.Trim().ToUpperInvariant();FullName=fullName.Trim();Email=email?.Trim().ToLowerInvariant();DepartmentId=departmentId;PositionId=positionId;}
    public Guid HotelId{get;private set;} public Guid? BranchId{get;private set;} public string EmployeeNumber{get;private set;}=string.Empty; public string FullName{get;private set;}=string.Empty;
    public string? Email{get;private set;} public Guid? DepartmentId{get;private set;} public Guid? PositionId{get;private set;} public bool IsActive{get;private set;}=true;
    public void Update(string fullName,string? email,Guid? departmentId,Guid? positionId){FullName=fullName.Trim();Email=email?.Trim().ToLowerInvariant();DepartmentId=departmentId;PositionId=positionId;MarkUpdated();}
}

using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Administration.Entities;
public sealed class AuditLog : BaseEntity
{
    private AuditLog(){} public AuditLog(Guid? hotelId,Guid? branchId,Guid? userId,string action,string entityName,string? entityId,string? oldValues,string? newValues,string? ipAddress,string? correlationId)
    {HotelId=hotelId;BranchId=branchId;UserId=userId;Action=action;EntityName=entityName;EntityId=entityId;OldValues=oldValues;NewValues=newValues;IpAddress=ipAddress;CorrelationId=correlationId;CreatedAtUtc=DateTimeOffset.UtcNow;}
    public Guid? HotelId{get;private set;} public Guid? BranchId{get;private set;} public Guid? UserId{get;private set;} public string Action{get;private set;}=string.Empty;
    public string EntityName{get;private set;}=string.Empty; public string? EntityId{get;private set;} public string? OldValues{get;private set;} public string? NewValues{get;private set;}
    public string? IpAddress{get;private set;} public string? CorrelationId{get;private set;} public DateTimeOffset CreatedAtUtc{get;private set;}
}

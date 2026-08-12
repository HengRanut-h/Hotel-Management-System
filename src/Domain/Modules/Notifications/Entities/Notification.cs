using HotelManagement.Domain.Common.Entities;
namespace HotelManagement.Domain.Modules.Notifications.Entities;
public sealed class Notification : AuditableEntity
{
    private Notification(){} public Notification(Guid hotelId,Guid? userId,string title,string message,string type="Info"){HotelId=hotelId;UserId=userId;Title=title.Trim();Message=message.Trim();Type=type.Trim();}
    public Guid HotelId{get;private set;} public Guid? UserId{get;private set;} public string Title{get;private set;}=string.Empty; public string Message{get;private set;}=string.Empty; public string Type{get;private set;}="Info";
    public bool IsRead{get;private set;} public DateTimeOffset? ReadAtUtc{get;private set;} public void MarkRead(){IsRead=true;ReadAtUtc=DateTimeOffset.UtcNow;MarkUpdated();}
}

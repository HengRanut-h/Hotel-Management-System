using HotelManagement.Domain.Common.Entities;
using HotelManagement.Domain.Modules.Rooms.Entities;

namespace HotelManagement.Domain.Modules.Maintenance.Entities;

public sealed class MaintenanceRequest : AuditableEntity
{
    private MaintenanceRequest() { }
    public MaintenanceRequest(Guid? roomId, string category, string description, string priority)
    {
        RoomId=roomId; Category=category.Trim(); Description=description.Trim(); Priority=priority.Trim(); Status="Open";
    }
    public Guid? RoomId { get; private set; }
    public Room? Room { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Priority { get; private set; } = "Normal";
    public string Status { get; private set; } = "Open";
    public Guid? AssignedUserId { get; private set; }
    public void Assign(Guid userId){ AssignedUserId=userId; Status="Assigned"; }
    public void Complete()=>Status="Completed";
}

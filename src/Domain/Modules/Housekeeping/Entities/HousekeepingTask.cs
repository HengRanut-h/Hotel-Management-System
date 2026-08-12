using HotelManagement.Domain.Common.Entities;
using HotelManagement.Domain.Modules.Rooms.Entities;

namespace HotelManagement.Domain.Modules.Housekeeping.Entities;

public sealed class HousekeepingTask : AuditableEntity
{
    private HousekeepingTask() { }
    public HousekeepingTask(Guid roomId, string taskType, string priority="Normal")
    {
        RoomId = roomId; TaskType = taskType.Trim(); Priority = priority.Trim(); Status = "Pending";
    }
    public Guid RoomId { get; private set; }
    public Room Room { get; private set; } = null!;
    public Guid? AssignedUserId { get; private set; }
    public string TaskType { get; private set; } = string.Empty;
    public string Priority { get; private set; } = "Normal";
    public string Status { get; private set; } = "Pending";
    public void Assign(Guid userId) { AssignedUserId=userId; Status="Assigned"; }
    public void Start()=>Status="InProgress";
    public void Complete()=>Status="Completed";
}

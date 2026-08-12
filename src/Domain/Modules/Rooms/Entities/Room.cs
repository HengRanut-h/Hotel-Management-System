using HotelManagement.Domain.Common.Entities;
using HotelManagement.Domain.Modules.Hotels.Entities;
using HotelManagement.Domain.Modules.Rooms.Enums;

namespace HotelManagement.Domain.Modules.Rooms.Entities;

public sealed class Room : AuditableEntity
{
    private Room() { }

    public Room(Guid hotelId, Guid branchId, Guid roomTypeId, string roomNumber, int floor)
    {
        HotelId = hotelId;
        BranchId = branchId;
        RoomTypeId = roomTypeId;
        RoomNumber = roomNumber.Trim();
        Floor = floor;
        Status = RoomStatus.Available;
    }

    public Guid HotelId { get; private set; }
    public Hotel Hotel { get; private set; } = null!;
    public Guid BranchId { get; private set; }
    public HotelBranch Branch { get; private set; } = null!;
    public Guid RoomTypeId { get; private set; }
    public RoomType RoomType { get; private set; } = null!;
    public string RoomNumber { get; private set; } = string.Empty;
    public int Floor { get; private set; }
    public RoomStatus Status { get; private set; }

    public void Update(Guid roomTypeId, string roomNumber, int floor)
    {
        RoomTypeId = roomTypeId;
        RoomNumber = roomNumber.Trim();
        Floor = floor;
    }

    public void ChangeStatus(RoomStatus status) => Status = status;
}

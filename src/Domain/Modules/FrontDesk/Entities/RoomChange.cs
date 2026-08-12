using HotelManagement.Domain.Common.Entities;
using HotelManagement.Domain.Common.Interfaces;

namespace HotelManagement.Domain.Modules.FrontDesk.Entities;

public sealed class RoomChange : AuditableEntity, IOperationalRecord
{
    private RoomChange() { }
    public RoomChange(Guid hotelId, Guid? branchId, string referenceNumber, string title, string status,
        string? notes, decimal amount, DateTimeOffset eventAtUtc, Guid? relatedEntityId = null, string? relatedEntityType = null)
    {
        HotelId = hotelId; BranchId = branchId; ReferenceNumber = referenceNumber.Trim().ToUpperInvariant();
        Title = title.Trim(); Status = status.Trim(); Notes = notes?.Trim(); Amount = amount; EventAtUtc = eventAtUtc;
        RelatedEntityId = relatedEntityId; RelatedEntityType = relatedEntityType?.Trim();
    }
    public Guid HotelId { get; private set; }
    public Guid? BranchId { get; private set; }
    public string ReferenceNumber { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Open";
    public string? Notes { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset EventAtUtc { get; private set; }
    public Guid? RelatedEntityId { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public void UpdateRecord(string title, string? notes, decimal amount, DateTimeOffset eventAtUtc, Guid? relatedEntityId, string? relatedEntityType)
    {
        Title = title.Trim(); Notes = notes?.Trim(); Amount = amount; EventAtUtc = eventAtUtc;
        RelatedEntityId = relatedEntityId; RelatedEntityType = relatedEntityType?.Trim(); MarkUpdated();
    }
    public void ChangeStatus(string status) { Status = status.Trim(); MarkUpdated(); }
}

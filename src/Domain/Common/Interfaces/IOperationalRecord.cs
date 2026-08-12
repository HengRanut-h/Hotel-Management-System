namespace HotelManagement.Domain.Common.Interfaces;

public interface IOperationalRecord
{
    Guid Id { get; }
    Guid HotelId { get; }
    Guid? BranchId { get; }
    string ReferenceNumber { get; }
    string Title { get; }
    string Status { get; }
    string? Notes { get; }
    decimal Amount { get; }
    DateTimeOffset EventAtUtc { get; }
    Guid? RelatedEntityId { get; }
    string? RelatedEntityType { get; }
    bool IsDeleted { get; }
    DateTimeOffset CreatedAtUtc { get; }

    void UpdateRecord(string title, string? notes, decimal amount, DateTimeOffset eventAtUtc, Guid? relatedEntityId,
        string? relatedEntityType);

    void ChangeStatus(string status);
    void SoftDelete(Guid? userId = null);
}
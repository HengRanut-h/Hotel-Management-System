namespace HotelManagement.Domain.Common.Interfaces;

public interface ICatalogEntity
{
    Guid Id { get; }
    Guid HotelId { get; }
    Guid? BranchId { get; }
    string Name { get; }
    string Code { get; }
    string? Description { get; }
    bool IsActive { get; }
    bool IsDeleted { get; }
    DateTimeOffset CreatedAtUtc { get; }

    void UpdateCatalog(string name, string code, string? description);
    void SetActive(bool active);
    void SoftDelete(Guid? userId = null);
    void Restore(Guid? userId = null);
}
using HotelManagement.Domain.Common.Entities;
using HotelManagement.Domain.Common.Interfaces;

namespace HotelManagement.Domain.Modules.Administration.Entities;

public sealed class SystemSetting : AuditableEntity, ICatalogEntity
{
    private SystemSetting() { }
    public SystemSetting(Guid hotelId, Guid? branchId, string name, string code, string? description = null)
    {
        HotelId = hotelId;
        BranchId = branchId;
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description?.Trim();
    }
    public Guid HotelId { get; private set; }
    public Guid? BranchId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public void UpdateCatalog(string name, string code, string? description)
    {
        Name = name.Trim(); Code = code.Trim().ToUpperInvariant(); Description = description?.Trim(); MarkUpdated();
    }
    public void SetActive(bool active) { IsActive = active; MarkUpdated(); }
}

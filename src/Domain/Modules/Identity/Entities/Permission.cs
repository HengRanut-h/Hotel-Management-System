using HotelManagement.Domain.Common.Entities;

namespace HotelManagement.Domain.Modules.Identity.Entities;

public sealed class Permission : AuditableEntity
{
    private Permission()
    {
    }

    public Permission(string name)
    {
        UpdateName(name);
    }

    public string Name { get; private set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; private set; }
        = new List<RolePermission>();

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Permission name is required.",
                nameof(name));
        }

        Name = name
            .Trim()
            .ToLowerInvariant();
    }
}
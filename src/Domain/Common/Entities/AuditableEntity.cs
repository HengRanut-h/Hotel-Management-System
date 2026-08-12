namespace HotelManagement.Domain.Common.Entities;

public abstract class AuditableEntity : BaseEntity
{
    public DateTimeOffset CreatedAtUtc { get; private set; } = DateTimeOffset.UtcNow;
    public Guid? CreatedBy { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public void MarkCreated(Guid? userId = null)
    {
        CreatedAtUtc = DateTimeOffset.UtcNow;
        CreatedBy = userId;
    }

    public void MarkUpdated(Guid? userId = null)
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedBy = userId;
    }

    public void SoftDelete(Guid? userId = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTimeOffset.UtcNow;
        DeletedBy = userId;
    }

    public void Restore(Guid? userId = null)
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        DeletedBy = null;
        MarkUpdated(userId);
    }
}
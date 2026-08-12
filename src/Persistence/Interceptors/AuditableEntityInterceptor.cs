using HotelManagement.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HotelManagement.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken ct = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private static void Apply(DbContext? db)
    {
        if (db is null) return;
        foreach (var e in db.ChangeTracker.Entries<AuditableEntity>())
        {
            if (e.State == EntityState.Added) e.Entity.MarkCreated();
            else if (e.State == EntityState.Modified) e.Entity.MarkUpdated();
        }
    }
}
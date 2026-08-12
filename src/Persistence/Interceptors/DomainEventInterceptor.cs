using HotelManagement.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HotelManagement.Persistence.Interceptors;

public sealed class DomainEventInterceptor : SaveChangesInterceptor
{
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        Clear(eventData.Context);
        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken ct = default)
    {
        Clear(eventData.Context);
        return base.SavedChangesAsync(eventData, result, ct);
    }

    private static void Clear(DbContext? db)
    {
        if (db is null) return;
        foreach (var e in db.ChangeTracker.Entries<AggregateRoot>()) e.Entity.ClearDomainEvents();
    }
}
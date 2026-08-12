using HotelManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Persistence.Repositories;

public sealed class Repository<TEntity>(ApplicationDbContext db) where TEntity : class
{
    public IQueryable<TEntity> Query() => db.Set<TEntity>().AsQueryable();

    public ValueTask<TEntity?> FindAsync(Guid id, CancellationToken ct = default) =>
        db.Set<TEntity>().FindAsync(new object?[] { id }, ct);

    public void Add(TEntity entity) => db.Set<TEntity>().Add(entity);
    public void Remove(TEntity entity) => db.Set<TEntity>().Remove(entity);
}
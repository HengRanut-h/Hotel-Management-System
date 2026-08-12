using HotelManagement.Domain.Modules.Hotels.Entities;
using HotelManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Persistence.Seeders;

public static class SystemDataSeeder
{
    public static async Task<(Hotel Hotel, HotelBranch Branch)> EnsureHotelAsync(ApplicationDbContext db,
        CancellationToken ct = default)
    {
        var h = await db.Hotels.FirstOrDefaultAsync(ct) ?? new Hotel("Demo Hotel", "DEMO", "USD");
        if (h.Id == Guid.Empty || db.Entry(h).State == EntityState.Detached)
        {
            db.Hotels.Add(h);
            await db.SaveChangesAsync(ct);
        }

        var b = await db.HotelBranches.FirstOrDefaultAsync(x => x.HotelId == h.Id, ct) ??
                new HotelBranch(h.Id, "Main Branch", "MAIN");
        if (b.Id == Guid.Empty || db.Entry(b).State == EntityState.Detached)
        {
            db.HotelBranches.Add(b);
            await db.SaveChangesAsync(ct);
        }

        return (h, b);
    }
}
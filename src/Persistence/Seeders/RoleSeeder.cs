using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Domain.Modules.Identity.Entities;
using HotelManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Persistence.Seeders;

public static class RoleSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken ct = default)
    {
        foreach (var name in SystemRoles.All)
        {
            var n = name.ToUpperInvariant();
            if (!await db.Roles.IgnoreQueryFilters().AnyAsync(x => x.NormalizedName == n, ct))
                db.Roles.Add(new Role(name));
        }

        await db.SaveChangesAsync(ct);
    }
}
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Domain.Modules.Identity.Entities;
using HotelManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Persistence.Seeders;

public static class PermissionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken ct = default)
    {
        foreach (var name in Permissions.All)
            if (!await db.Permissions.IgnoreQueryFilters().AnyAsync(x => x.Name == name, ct))
                db.Permissions.Add(new Permission(name));
        await db.SaveChangesAsync(ct);
    }
}
using HotelManagement.Domain.Modules.Identity.Entities;
using HotelManagement.Persistence.Context;
using Microsoft.AspNetCore.Identity;

namespace HotelManagement.Persistence.Seeders;

public static class AdminSeeder
{
    public static async Task EnsurePasswordAsync(ApplicationDbContext db, User admin, string password,
        IPasswordHasher<User> hasher, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(admin.PasswordHash))
        {
            admin.SetPasswordHash(hasher.HashPassword(admin, password));
            await db.SaveChangesAsync(ct);
        }
    }
}
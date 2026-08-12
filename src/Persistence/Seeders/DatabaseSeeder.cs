using HotelManagement.Domain.Modules.Hotels.Entities;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Domain.Modules.Identity.Entities;
using HotelManagement.Domain.Modules.Rooms.Entities;
using HotelManagement.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Persistence.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext db,
        IConfiguration configuration,
        IPasswordHasher<User> passwordHasher,
        CancellationToken ct = default)
    {
        foreach (var permissionName in Permissions.All)
        {
            if (!await db.Permissions
                    .IgnoreQueryFilters()
                    .AnyAsync(
                        x => x.Name == permissionName,
                        ct))
            {
                db.Permissions.Add(
                    new Permission(permissionName));
            }
        }

        await db.SaveChangesAsync(ct);

        foreach (var roleName in SystemRoles.All)
        {
            var normalized =
                roleName.ToUpperInvariant();

            if (!await db.Roles
                    .IgnoreQueryFilters()
                    .AnyAsync(
                        x => x.NormalizedName == normalized,
                        ct))
            {
                db.Roles.Add(
                    new Role(roleName));
            }
        }

        await db.SaveChangesAsync(ct);

        var roles =
            await db.Roles
                .ToDictionaryAsync(
                    x => x.Name,
                    StringComparer.OrdinalIgnoreCase,
                    ct);

        var superAdminRole = roles[SystemRoles.SuperAdmin];

        var allPermissions =
            await db.Permissions
                .ToDictionaryAsync(
                    x => x.Name,
                    StringComparer.OrdinalIgnoreCase,
                    ct);

        static string[] RolePermissions(
            string roleName) =>
            roleName switch
            {
                SystemRoles.SuperAdmin =>
                    Permissions.All,

                SystemRoles.HotelAdmin =>
                    Permissions.All,

                SystemRoles.Manager =>
                [
                    Permissions.Dashboard.View,
                    Permissions.Rooms.View,
                    Permissions.Rooms.Update,
                    Permissions.Rooms.ChangeStatus,
                    Permissions.Guests.View,
                    Permissions.Guests.Create,
                    Permissions.Guests.Update,
                    Permissions.Reservations.View,
                    Permissions.Reservations.Create,
                    Permissions.Reservations.Cancel,
                    Permissions.Reservations.CheckIn,
                    Permissions.Reservations.CheckOut,
                    Permissions.Invoices.View,
                    Permissions.Invoices.Create,
                    Permissions.Payments.View,
                    Permissions.Payments.Create,
                    Permissions.Payments.Refund,
                    Permissions.Utilities.View,
                    Permissions.Utilities.RecordReading,
                    Permissions.Housekeeping.View,
                    Permissions.Housekeeping.Manage,
                    Permissions.Maintenance.View,
                    Permissions.Maintenance.Manage,
                    Permissions.Administration.AuditView
                ],

                SystemRoles.Receptionist =>
                [
                    Permissions.Dashboard.View,
                    Permissions.Rooms.View,
                    Permissions.Guests.View,
                    Permissions.Guests.Create,
                    Permissions.Guests.Update,
                    Permissions.Reservations.View,
                    Permissions.Reservations.Create,
                    Permissions.Reservations.Cancel,
                    Permissions.Reservations.CheckIn,
                    Permissions.Reservations.CheckOut,
                    Permissions.Invoices.View,
                    Permissions.Invoices.Create,
                    Permissions.Payments.View,
                    Permissions.Payments.Create,
                    Permissions.Utilities.View,
                    Permissions.Housekeeping.View,
                    Permissions.Maintenance.View
                ],

                SystemRoles.Accountant =>
                [
                    Permissions.Dashboard.View,
                    Permissions.Guests.View,
                    Permissions.Reservations.View,
                    Permissions.Invoices.View,
                    Permissions.Invoices.Create,
                    Permissions.Payments.View,
                    Permissions.Payments.Create,
                    Permissions.Payments.Refund,
                    Permissions.Utilities.View
                ],

                SystemRoles.Housekeeper =>
                [
                    Permissions.Dashboard.View,
                    Permissions.Rooms.View,
                    Permissions.Housekeeping.View,
                    Permissions.Housekeeping.Manage,
                    Permissions.Utilities.View,
                    Permissions.Utilities.RecordReading
                ],

                SystemRoles.Technician =>
                [
                    Permissions.Dashboard.View,
                    Permissions.Rooms.View,
                    Permissions.Maintenance.View,
                    Permissions.Maintenance.Manage
                ],

                SystemRoles.Guest =>
                [
                    Permissions.Dashboard.View
                ],

                _ => []
            };

        foreach (var role in roles.Values)
        {
            foreach (var permissionName in RolePermissions(role.Name))
            {
                if (!allPermissions.TryGetValue(
                        permissionName,
                        out var permission))
                {
                    continue;
                }

                if (!await db.RolePermissions.AnyAsync(
                        x =>
                            x.RoleId == role.Id &&
                            x.PermissionId == permission.Id,
                        ct))
                {
                    db.RolePermissions.Add(
                        new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = permission.Id
                        });
                }
            }
        }

        await db.SaveChangesAsync(ct);

        var hotel =
            await db.Hotels.FirstOrDefaultAsync(ct);

        if (hotel is null)
        {
            hotel = new Hotel(
                "Demo Hotel",
                "DEMO",
                "USD");

            db.Hotels.Add(hotel);
            await db.SaveChangesAsync(ct);
        }

        var branch =
            await db.HotelBranches.FirstOrDefaultAsync(
                x => x.HotelId == hotel.Id,
                ct);

        if (branch is null)
        {
            branch = new HotelBranch(
                hotel.Id,
                "Main Branch",
                "MAIN");

            db.HotelBranches.Add(branch);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.RoomTypes.AnyAsync(
                x => x.HotelId == hotel.Id,
                ct))
        {
            db.RoomTypes.AddRange(
                new RoomType(
                    hotel.Id,
                    "Standard",
                    "STD",
                    35m,
                    2,
                    1),

                new RoomType(
                    hotel.Id,
                    "Deluxe",
                    "DLX",
                    65m,
                    2,
                    2),

                new RoomType(
                    hotel.Id,
                    "Suite",
                    "STE",
                    120m,
                    4,
                    2));

            await db.SaveChangesAsync(ct);
        }

        var adminEmail =
            (configuration["Seed:AdminEmail"] ??
             "admin@hotel.local")
            .Trim()
            .ToLowerInvariant();

        var admin =
            await db.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x => x.Email == adminEmail,
                    ct);

        if (admin is null)
        {
            admin = new User(
                configuration["Seed:AdminFullName"] ??
                "System Administrator",
                adminEmail);

            admin.SetScope(
                hotel.Id,
                branch.Id);

            var password =
                configuration["Seed:AdminPassword"] ??
                "ChangeMe123!";

            admin.SetPasswordHash(
                passwordHasher.HashPassword(
                    admin,
                    password));

            db.Users.Add(admin);
            await db.SaveChangesAsync(ct);

            db.UserRoles.Add(
                new UserRole
                {
                    UserId = admin.Id,
                    RoleId = superAdminRole.Id
                });

            await db.SaveChangesAsync(ct);
        }
    }
}

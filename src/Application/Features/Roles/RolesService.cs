using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Modules.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Roles;

public sealed record RoleRequest(
    string Name);

public sealed record RoleResponse(
    Guid Id,
    string Name,
    List<string> Permissions,
    int UserCount);

public sealed record SetRolePermissionsRequest(
    List<string> Permissions);

public sealed class RolesService(
    IApplicationDbContext db)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<List<RoleResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var roles =
            await db.Roles
                .AsNoTracking()
                .Include(
                    role =>
                        role.RolePermissions)
                .ThenInclude(
                    rolePermission =>
                        rolePermission.Permission)
                .Include(
                    role =>
                        role.UserRoles)
                .Where(
                    role =>
                        !role.IsDeleted)
                .OrderBy(
                    role =>
                        role.Name)
                .ToListAsync(
                    cancellationToken);

        return roles
            .Select(
                Map)
            .ToList();
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<RoleResponse> GetAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var role =
            await db.Roles
                .AsNoTracking()
                .Include(
                    role =>
                        role.RolePermissions)
                .ThenInclude(
                    rolePermission =>
                        rolePermission.Permission)
                .Include(
                    role =>
                        role.UserRoles)
                .FirstOrDefaultAsync(
                    role =>
                        role.Id == id
                        &&
                        !role.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Role not found.");

        return Map(
            role);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<RoleResponse> CreateAsync(
        RoleRequest request,
        CancellationToken cancellationToken)
    {
        var name =
            request.Name
                .Trim();

        if (
            string.IsNullOrWhiteSpace(
                name))
        {
            throw new ArgumentException(
                "Role name is required.",
                nameof(request));
        }

        var normalizedName =
            name
                .ToUpperInvariant();

        var roleExists =
            await db.Roles
                .AnyAsync(
                    role =>
                        role.NormalizedName == normalizedName
                        &&
                        !role.IsDeleted,
                    cancellationToken);

        if (
            roleExists)
        {
            throw new ConflictException(
                "Role already exists.");
        }

        var role =
            new Role(
                name);

        db.Roles.Add(
            role);

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetAsync(
            role.Id,
            cancellationToken);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<RoleResponse> UpdateAsync(
        Guid id,
        RoleRequest request,
        CancellationToken cancellationToken)
    {
        var role =
            await db.Roles
                .FirstOrDefaultAsync(
                    role =>
                        role.Id == id
                        &&
                        !role.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Role not found.");

        var name =
            request.Name
                .Trim();

        if (
            string.IsNullOrWhiteSpace(
                name))
        {
            throw new ArgumentException(
                "Role name is required.",
                nameof(request));
        }

        var normalizedName =
            name
                .ToUpperInvariant();

        var duplicateExists =
            await db.Roles
                .AnyAsync(
                    existingRole =>
                        existingRole.Id != id
                        &&
                        existingRole.NormalizedName == normalizedName
                        &&
                        !existingRole.IsDeleted,
                    cancellationToken);

        if (
            duplicateExists)
        {
            throw new ConflictException(
                "Role already exists.");
        }

        role.Update(
            name);

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetAsync(
            id,
            cancellationToken);
    }

    // =========================================================
    // DELETE
    //
    // Role with permissions:
    // ALLOWED
    //
    // Role assigned to users:
    // BLOCKED
    // =========================================================

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // LOAD ROLE RELATIONSHIPS
        // =====================================================

        var role =
            await db.Roles
                .Include(
                    role =>
                        role.RolePermissions)
                .Include(
                    role =>
                        role.UserRoles)
                .FirstOrDefaultAsync(
                    role =>
                        role.Id == id
                        &&
                        !role.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Role not found.");

        // =====================================================
        // DON'T DELETE ROLE ASSIGNED TO USERS
        // =====================================================

        if (
            role.UserRoles.Count > 0)
        {
            throw new ConflictException(
                "Cannot delete a role that is assigned to users. " +
                "Remove the role from all users first.");
        }

        // =====================================================
        // REMOVE ROLE PERMISSIONS
        //
        // A role may be deleted even when it currently has
        // permissions assigned.
        // =====================================================

        if (
            role.RolePermissions.Count > 0)
        {
            db.RolePermissions
                .RemoveRange(
                    role.RolePermissions);
        }

        // =====================================================
        // SOFT DELETE ROLE
        // =====================================================

        role.SoftDelete();

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // SET PERMISSIONS
    // =========================================================

    public async Task SetPermissionsAsync(
        Guid id,
        SetRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        var role =
            await db.Roles
                .Include(
                    role =>
                        role.RolePermissions)
                .FirstOrDefaultAsync(
                    role =>
                        role.Id == id
                        &&
                        !role.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Role not found.");

        var permissionNames =
            request.Permissions
                .Where(
                    permission =>
                        !string.IsNullOrWhiteSpace(
                            permission))
                .Select(
                    permission =>
                        permission.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        var permissions =
            await db.Permissions
                .Where(
                    permission =>
                        permissionNames.Contains(
                            permission.Name)
                        &&
                        !permission.IsDeleted)
                .ToListAsync(
                    cancellationToken);

        var missingPermissions =
            permissionNames
                .Except(
                    permissions.Select(
                        permission =>
                            permission.Name),
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        if (
            missingPermissions.Count > 0)
        {
            throw new ConflictException(
                "Unknown permissions: "
                +
                string.Join(
                    ", ",
                    missingPermissions));
        }

        // =====================================================
        // REMOVE OLD PERMISSIONS
        // =====================================================

        db.RolePermissions
            .RemoveRange(
                role.RolePermissions);

        // =====================================================
        // ADD NEW PERMISSIONS
        // =====================================================

        foreach (
            var permission
            in permissions)
        {
            db.RolePermissions.Add(
                new RolePermission
                {
                    RoleId =
                        role.Id,

                    PermissionId =
                        permission.Id
                });
        }

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // MAPPING
    // =========================================================

    private static RoleResponse Map(
        Role role)
    {
        return new RoleResponse(
            role.Id,
            role.Name,
            role.RolePermissions
                .Select(
                    rolePermission =>
                        rolePermission.Permission.Name)
                .Order()
                .ToList(),
            role.UserRoles.Count);
    }
}
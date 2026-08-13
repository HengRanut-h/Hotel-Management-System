using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Users.Contracts;
using HotelManagement.Domain.Modules.Identity.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Users.Services;

public sealed class UserAdminService(
    IApplicationDbContext db,
    IPasswordHasher<User> passwordHasher)
{
    // =========================================================
    // GET ALL
    //
    // Security:
    //
    // Permission is checked by Controller.
    //
    // Here:
    // 1. Hotel scope
    // 2. Exclude current user
    // 3. Role hierarchy
    // 4. Portal/domain hierarchy
    // =========================================================

    public async Task<List<UserResponse>>
        GetAllAsync(
            Guid? hotelId,
            IReadOnlyCollection<string> actorRoles,
            Guid? actorUserId,
            CancellationToken cancellationToken)
    {
        IQueryable<User> query =
            db.Users
                .AsNoTracking()
                .Include(
                    user =>
                        user.UserRoles)
                .ThenInclude(
                    userRole =>
                        userRole.Role)
                .Where(
                    user =>
                        !user.IsDeleted);

        query =
            ApplyHotelScope(
                query,
                hotelId,
                actorRoles);

        if (actorUserId.HasValue)
        {
            query =
                query.Where(
                    user =>
                        user.Id !=
                        actorUserId.Value);
        }

        var users =
            await query
                .OrderBy(
                    user =>
                        user.FullName)
                .ToListAsync(
                    cancellationToken);

        return users
            .Where(
                user =>
                    RoleAccessPolicy
                        .CanManageUser(
                            actorRoles,
                            GetRoleNames(user)))
            .Select(Map)
            .ToList();
    }

    // =========================================================
    // ASSIGNABLE ROLES
    // =========================================================

    public async Task<List<string>>
        GetAssignableRolesAsync(
            IReadOnlyCollection<string> actorRoles,
            CancellationToken cancellationToken)
    {
        var roles =
            await db.Roles
                .AsNoTracking()
                .Where(
                    role =>
                        !role.IsDeleted)
                .OrderBy(
                    role =>
                        role.Name)
                .Select(
                    role =>
                        role.Name)
                .ToListAsync(
                    cancellationToken);

        return roles
            .Where(
                roleName =>
                    RoleAccessPolicy
                        .CanAssignRoles(
                            actorRoles,
                            [roleName]))
            .ToList();
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<UserResponse>
        GetByIdAsync(
            Guid? hotelId,
            IReadOnlyCollection<string> actorRoles,
            Guid? actorUserId,
            Guid id,
            CancellationToken cancellationToken)
    {
        IQueryable<User> query =
            db.Users
                .AsNoTracking()
                .Include(
                    user =>
                        user.UserRoles)
                .ThenInclude(
                    userRole =>
                        userRole.Role)
                .Where(
                    user =>
                        user.Id == id
                        &&
                        !user.IsDeleted);

        query =
            ApplyHotelScope(
                query,
                hotelId,
                actorRoles);

        var user =
            await query
                .FirstOrDefaultAsync(
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

        EnsureCanManageTarget(
            actorRoles,
            actorUserId,
            user);

        return Map(
            user);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<UserResponse>
        CreateAsync(
            Guid hotelId,
            Guid branchId,
            IReadOnlyCollection<string> actorRoles,
            CreateUserRequest request,
            CancellationToken cancellationToken)
    {
        var email =
            NormalizeEmail(
                request.Email);

        // =====================================================
        // EMAIL
        // =====================================================

        var emailExists =
            await db.Users
                .IgnoreQueryFilters()
                .AnyAsync(
                    user =>
                        user.Email == email,
                    cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                "Email already exists.");
        }

        // =====================================================
        // ROLES
        // =====================================================

        var roles =
            await ResolveRolesAsync(
                request.RoleNames,
                cancellationToken);

        if (roles.Count == 0)
        {
            throw new ConflictException(
                "At least one role is required.");
        }

        EnsureCanAssignRoles(
            actorRoles,
            roles);

        // =====================================================
        // CREATE USER
        // =====================================================

        var user =
            new User(
                request.FullName.Trim(),
                email);

        user.SetScope(
            hotelId,
            request.BranchId
            ??
            branchId);

        user.SetPasswordHash(
            passwordHasher.HashPassword(
                user,
                request.Password));

        db.Users.Add(
            user);

        await db.SaveChangesAsync(
            cancellationToken);

        // =====================================================
        // USER ROLES
        // =====================================================

        foreach (
            var role in roles)
        {
            db.UserRoles.Add(
                new UserRole
                {
                    UserId =
                        user.Id,

                    RoleId =
                        role.Id
                });
        }

        await db.SaveChangesAsync(
            cancellationToken);

        // We know the actor can assign these roles,
        // so map directly after loading relationships.
        var created =
            await db.Users
                .AsNoTracking()
                .Include(
                    item =>
                        item.UserRoles)
                .ThenInclude(
                    userRole =>
                        userRole.Role)
                .FirstAsync(
                    item =>
                        item.Id ==
                        user.Id,
                    cancellationToken);

        return Map(
            created);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<UserResponse>
        UpdateAsync(
            Guid? hotelId,
            IReadOnlyCollection<string> actorRoles,
            Guid? actorUserId,
            Guid id,
            UpdateUserRequest request,
            CancellationToken cancellationToken)
    {
        IQueryable<User> query =
            db.Users
                .Include(
                    user =>
                        user.UserRoles)
                .ThenInclude(
                    userRole =>
                        userRole.Role)
                .Where(
                    user =>
                        user.Id == id
                        &&
                        !user.IsDeleted);

        query =
            ApplyHotelScope(
                query,
                hotelId,
                actorRoles);

        var user =
            await query
                .FirstOrDefaultAsync(
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

        // =====================================================
        // TARGET SECURITY
        // =====================================================

        EnsureCanManageTarget(
            actorRoles,
            actorUserId,
            user);

        // =====================================================
        // EMAIL
        // =====================================================

        var email =
            NormalizeEmail(
                request.Email);

        var emailExists =
            await db.Users
                .IgnoreQueryFilters()
                .AnyAsync(
                    existingUser =>
                        existingUser.Id != id
                        &&
                        existingUser.Email ==
                        email,
                    cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                "Email already exists.");
        }

        // =====================================================
        // NEW ROLES
        // =====================================================

        var roles =
            await ResolveRolesAsync(
                request.RoleNames,
                cancellationToken);

        if (roles.Count == 0)
        {
            throw new ConflictException(
                "At least one role is required.");
        }

        EnsureCanAssignRoles(
            actorRoles,
            roles);

        // =====================================================
        // PROFILE
        // =====================================================

        user.UpdateProfile(
            request.FullName.Trim(),
            email);

        // Preserve current HotelId.
        if (user.HotelId.HasValue)
        {
            user.SetScope(
                user.HotelId.Value,
                request.BranchId);
        }

        // =====================================================
        // STATUS
        // =====================================================

        if (request.IsActive)
        {
            user.Enable();
        }
        else
        {
            user.Disable();
        }

        // =====================================================
        // REPLACE ROLES
        // =====================================================

        db.UserRoles.RemoveRange(
            user.UserRoles);

        foreach (
            var role in roles)
        {
            db.UserRoles.Add(
                new UserRole
                {
                    UserId =
                        user.Id,

                    RoleId =
                        role.Id
                });
        }

        await db.SaveChangesAsync(
            cancellationToken);

        var updated =
            await db.Users
                .AsNoTracking()
                .Include(
                    item =>
                        item.UserRoles)
                .ThenInclude(
                    userRole =>
                        userRole.Role)
                .FirstAsync(
                    item =>
                        item.Id == id,
                    cancellationToken);

        return Map(
            updated);
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    public async Task SetActiveAsync(
        Guid? hotelId,
        IReadOnlyCollection<string> actorRoles,
        Guid? actorUserId,
        Guid id,
        bool isActive,
        CancellationToken cancellationToken)
    {
        IQueryable<User> query =
            db.Users
                .Include(
                    user =>
                        user.UserRoles)
                .ThenInclude(
                    userRole =>
                        userRole.Role)
                .Where(
                    user =>
                        user.Id == id
                        &&
                        !user.IsDeleted);

        query =
            ApplyHotelScope(
                query,
                hotelId,
                actorRoles);

        var user =
            await query
                .FirstOrDefaultAsync(
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

        EnsureCanManageTarget(
            actorRoles,
            actorUserId,
            user);

        if (isActive)
        {
            user.Enable();
        }
        else
        {
            user.Disable();
        }

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // RESET PASSWORD
    // =========================================================

    public async Task ResetPasswordAsync(
        Guid? hotelId,
        IReadOnlyCollection<string> actorRoles,
        Guid? actorUserId,
        Guid id,
        string password,
        CancellationToken cancellationToken)
    {
        IQueryable<User> query =
            db.Users
                .Include(
                    user =>
                        user.UserRoles)
                .ThenInclude(
                    userRole =>
                        userRole.Role)
                .Where(
                    user =>
                        user.Id == id
                        &&
                        !user.IsDeleted);

        query =
            ApplyHotelScope(
                query,
                hotelId,
                actorRoles);

        var user =
            await query
                .FirstOrDefaultAsync(
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

        EnsureCanManageTarget(
            actorRoles,
            actorUserId,
            user);

        if (string.IsNullOrWhiteSpace(
                password))
        {
            throw new ConflictException(
                "Password is required.");
        }

        user.SetPasswordHash(
            passwordHasher.HashPassword(
                user,
                password));

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // DELETE
    // =========================================================

    public async Task DeleteAsync(
        Guid? hotelId,
        IReadOnlyCollection<string> actorRoles,
        Guid? actorUserId,
        Guid id,
        CancellationToken cancellationToken)
    {
        IQueryable<User> query =
            db.Users
                .Include(
                    user =>
                        user.UserRoles)
                .ThenInclude(
                    userRole =>
                        userRole.Role)
                .Where(
                    user =>
                        user.Id == id
                        &&
                        !user.IsDeleted);

        query =
            ApplyHotelScope(
                query,
                hotelId,
                actorRoles);

        var user =
            await query
                .FirstOrDefaultAsync(
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

        EnsureCanManageTarget(
            actorRoles,
            actorUserId,
            user);

        user.SoftDelete();

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // HOTEL SCOPE
    // =========================================================

    private static IQueryable<User>
        ApplyHotelScope(
            IQueryable<User> query,
            Guid? hotelId,
            IReadOnlyCollection<string> actorRoles)
    {
        if (
            RoleAccessPolicy
                .IsSuperAdmin(
                    actorRoles))
        {
            return query;
        }

        if (!hotelId.HasValue)
        {
            throw new ForbiddenException(
                "Hotel context is required.");
        }

        return query.Where(
            user =>
                user.HotelId ==
                hotelId.Value);
    }

    // =========================================================
    // TARGET ACCESS
    // =========================================================

    private static void
        EnsureCanManageTarget(
            IReadOnlyCollection<string> actorRoles,
            Guid? actorUserId,
            User target)
    {
        // Never manage yourself from admin endpoints.
        if (
            actorUserId.HasValue
            &&
            target.Id ==
            actorUserId.Value)
        {
            throw new ForbiddenException(
                "You cannot manage your own account from this administration action.");
        }

        if (
            !RoleAccessPolicy
                .CanManageUser(
                    actorRoles,
                    GetRoleNames(
                        target)))
        {
            throw new ForbiddenException(
                "You are not allowed to manage this user.");
        }
    }

    // =========================================================
    // ASSIGN ROLE SECURITY
    // =========================================================

    private static void
        EnsureCanAssignRoles(
            IReadOnlyCollection<string> actorRoles,
            IReadOnlyCollection<Role> roles)
    {
        var roleNames =
            roles
                .Select(
                    role =>
                        role.Name)
                .ToList();

        if (
            !RoleAccessPolicy
                .CanAssignRoles(
                    actorRoles,
                    roleNames))
        {
            throw new ForbiddenException(
                "You are not allowed to assign one or more of the selected roles.");
        }
    }

    // =========================================================
    // RESOLVE ROLES
    // =========================================================

    private async Task<List<Role>>
        ResolveRolesAsync(
            List<string> roleNames,
            CancellationToken cancellationToken)
    {
        var normalizedNames =
            roleNames
                .Where(
                    roleName =>
                        !string.IsNullOrWhiteSpace(
                            roleName))
                .Select(
                    roleName =>
                        roleName
                            .Trim()
                            .ToUpperInvariant())
                .Distinct()
                .ToList();

        var roles =
            await db.Roles
                .Where(
                    role =>
                        normalizedNames
                            .Contains(
                                role.NormalizedName)
                        &&
                        !role.IsDeleted)
                .ToListAsync(
                    cancellationToken);

        if (
            roles.Count !=
            normalizedNames.Count)
        {
            throw new NotFoundException(
                "One or more roles were not found.");
        }

        return roles;
    }

    // =========================================================
    // ROLE NAMES
    // =========================================================

    private static IReadOnlyCollection<string>
        GetRoleNames(
            User user)
    {
        return user.UserRoles
            .Where(
                userRole =>
                    userRole.Role is not null
                    &&
                    !userRole.Role
                        .IsDeleted)
            .Select(
                userRole =>
                    userRole.Role.Name)
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    // =========================================================
    // NORMALIZE EMAIL
    // =========================================================

    private static string NormalizeEmail(
        string email)
    {
        return email
            .Trim()
            .ToLowerInvariant();
    }

    // =========================================================
    // MAP
    // =========================================================

    private static UserResponse Map(
        User user)
    {
        return new UserResponse
        {
            Id =
                user.Id,

            FullName =
                user.FullName,

            Email =
                user.Email,

            IsActive =
                user.IsActive,

            HotelId =
                user.HotelId,

            BranchId =
                user.BranchId,

            Roles =
                GetRoleNames(
                    user)
                    .Order()
                    .ToList()
        };
    }
}
using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
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
    // =========================================================

    public async Task<List<UserResponse>> GetAllAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        var users =
            await db.Users
                .AsNoTracking()
                .Include(
                    user =>
                        user.UserRoles)
                .ThenInclude(
                    userRole =>
                        userRole.Role)
                .Where(
                    user =>
                        user.HotelId == hotelId
                        &&
                        !user.IsDeleted)
                .OrderBy(
                    user =>
                        user.FullName)
                .ToListAsync(
                    cancellationToken);

        return users
            .Select(Map)
            .ToList();
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<UserResponse> GetByIdAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var user =
            await db.Users
                .AsNoTracking()
                .Include(
                    user =>
                        user.UserRoles)
                .ThenInclude(
                    userRole =>
                        userRole.Role)
                .FirstOrDefaultAsync(
                    user =>
                        user.HotelId == hotelId
                        &&
                        user.Id == id
                        &&
                        !user.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

        return Map(
            user);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<UserResponse> CreateAsync(
        Guid hotelId,
        Guid branchId,
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var email =
            NormalizeEmail(
                request.Email);

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

        var roles =
            await ResolveRolesAsync(
                request.RoleNames,
                cancellationToken);

        var user =
            new User(
                request.FullName.Trim(),
                email);

        user.SetScope(
            hotelId,
            request.BranchId
            ?? branchId);

        user.SetPasswordHash(
            passwordHasher.HashPassword(
                user,
                request.Password));

        db.Users.Add(
            user);

        await db.SaveChangesAsync(
            cancellationToken);

        foreach (var role in roles)
        {
            db.UserRoles.Add(
                new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
        }

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetByIdAsync(
            hotelId,
            user.Id,
            cancellationToken);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<UserResponse> UpdateAsync(
        Guid hotelId,
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user =
            await db.Users
                .Include(
                    user =>
                        user.UserRoles)
                .FirstOrDefaultAsync(
                    user =>
                        user.HotelId == hotelId
                        &&
                        user.Id == id
                        &&
                        !user.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

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
                        existingUser.Email == email,
                    cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                "Email already exists.");
        }

        var roles =
            await ResolveRolesAsync(
                request.RoleNames,
                cancellationToken);

        user.UpdateProfile(
            request.FullName.Trim(),
            email);

        user.SetScope(
            hotelId,
            request.BranchId);

        if (request.IsActive)
        {
            user.Enable();
        }
        else
        {
            user.Disable();
        }

        db.UserRoles.RemoveRange(
            user.UserRoles);

        foreach (var role in roles)
        {
            db.UserRoles.Add(
                new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
        }

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetByIdAsync(
            hotelId,
            id,
            cancellationToken);
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    public async Task SetActiveAsync(
        Guid hotelId,
        Guid id,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var user =
            await db.Users
                .FirstOrDefaultAsync(
                    user =>
                        user.HotelId == hotelId
                        &&
                        user.Id == id
                        &&
                        !user.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

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
    // DISABLE
    // =========================================================

    public Task DisableAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken) =>
        SetActiveAsync(
            hotelId,
            id,
            false,
            cancellationToken);

    // =========================================================
    // RESET PASSWORD
    // =========================================================

    public async Task ResetPasswordAsync(
        Guid hotelId,
        Guid id,
        string password,
        CancellationToken cancellationToken)
    {
        var user =
            await db.Users
                .FirstOrDefaultAsync(
                    user =>
                        user.HotelId == hotelId
                        &&
                        user.Id == id
                        &&
                        !user.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

        if (string.IsNullOrWhiteSpace(password))
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
    // SOFT DELETE
    // =========================================================

    public async Task DeleteAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var user =
            await db.Users
                .FirstOrDefaultAsync(
                    user =>
                        user.HotelId == hotelId
                        &&
                        user.Id == id
                        &&
                        !user.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

        user.SoftDelete();

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // GET AVAILABLE ROLES
    // =========================================================

    public Task<List<string>> GetRolesAsync(
        CancellationToken cancellationToken) =>
        db.Roles
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

    // =========================================================
    // RESOLVE ROLES
    // =========================================================

    private async Task<List<Role>> ResolveRolesAsync(
        List<string> roleNames,
        CancellationToken cancellationToken)
    {
        var normalizedNames =
            roleNames
                .Where(
                    roleName =>
                        !string.IsNullOrWhiteSpace(roleName))
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
                        normalizedNames.Contains(
                            role.NormalizedName)
                        &&
                        !role.IsDeleted)
                .ToListAsync(
                    cancellationToken);

        if (roles.Count != normalizedNames.Count)
        {
            throw new NotFoundException(
                "One or more roles were not found.");
        }

        return roles;
    }

    // =========================================================
    // NORMALIZE EMAIL
    // =========================================================

    private static string NormalizeEmail(
        string email) =>
        email
            .Trim()
            .ToLowerInvariant();

    // =========================================================
    // MAPPING
    // =========================================================

    private static UserResponse Map(
        User user) =>
        new()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            IsActive = user.IsActive,
            HotelId = user.HotelId,
            BranchId = user.BranchId,

            Roles =
                user.UserRoles
                    .Where(
                        userRole =>
                            !userRole.Role.IsDeleted)
                    .Select(
                        userRole =>
                            userRole.Role.Name)
                    .Distinct()
                    .Order()
                    .ToList()
        };
}
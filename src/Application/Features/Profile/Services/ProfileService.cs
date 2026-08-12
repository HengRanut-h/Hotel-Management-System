using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Features.Users.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Profile.Services;

public sealed class ProfileService(
    IApplicationDbContext db)
{
    // =========================================================
    // GET PROFILE
    // =========================================================

    public async Task<UserResponse> GetAsync(
        Guid userId,
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
                        user.Id == userId
                        &&
                        !user.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "User not found.");

        return new UserResponse
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
                    .OrderBy(
                        roleName =>
                            roleName)
                    .ToList()
        };
    }
}
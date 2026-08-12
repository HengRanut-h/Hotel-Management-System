using HotelManagement.Application.Abstractions.Authentication;
using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Features.Authentication.Contracts;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Domain.Modules.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Authentication.Services;

public sealed class AuthService(
    IApplicationDbContext db,
    IJwtTokenService jwtTokenService,
    IPasswordHasher<User> passwordHasher)
{
    // =========================================================
    // REGISTER
    // =========================================================

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var fullName = request.FullName.Trim();

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        // -----------------------------------------------------
        // 1. Check duplicate email
        // -----------------------------------------------------

        var emailExists = await db.Users
            .AnyAsync(
                x => x.Email == email,
                cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                "An account with this email already exists.");
        }

        // -----------------------------------------------------
        // 2. Find default Guest role
        // -----------------------------------------------------

        var guestRole = await db.Roles
            .FirstOrDefaultAsync(
                x => x.Name == SystemRoles.Guest,
                cancellationToken);

        if (guestRole is null)
        {
            throw new InvalidOperationException(
                $"Default role '{SystemRoles.Guest}' is not configured.");
        }

        // -----------------------------------------------------
        // 3. Create user
        // -----------------------------------------------------

        var user = new User(
            fullName,
            email);

        // -----------------------------------------------------
        // 4. Hash password
        // -----------------------------------------------------

        var passwordHash =
            passwordHasher.HashPassword(
                user,
                request.Password);

        user.SetPasswordHash(passwordHash);

        // -----------------------------------------------------
        // 5. Add user
        // -----------------------------------------------------

        db.Users.Add(user);

        // -----------------------------------------------------
        // 6. Assign default Guest role
        // -----------------------------------------------------

        db.UserRoles.Add(
            new UserRole
            {
                UserId = user.Id,
                RoleId = guestRole.Id
            });

        await db.SaveChangesAsync(cancellationToken);

        // -----------------------------------------------------
        // 7. Reload user with roles + permissions
        // -----------------------------------------------------

        var registeredUser = await db.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .ThenInclude(x => x.RolePermissions)
            .ThenInclude(x => x.Permission)
            .FirstAsync(
                x => x.Id == user.Id,
                cancellationToken);

        // -----------------------------------------------------
        // 8. Generate JWT + Refresh Token
        // -----------------------------------------------------

        var pair =
            await jwtTokenService.CreateTokenPairAsync(
                registeredUser,
                cancellationToken);

        // -----------------------------------------------------
        // 9. Store hashed refresh token
        // -----------------------------------------------------

        db.RefreshTokens.Add(
            new RefreshToken(
                registeredUser.Id,
                jwtTokenService.HashRefreshToken(
                    pair.RefreshToken),
                pair.RefreshTokenExpiresAtUtc));

        await db.SaveChangesAsync(cancellationToken);

        // -----------------------------------------------------
        // 10. Return authentication response
        // -----------------------------------------------------

        return BuildResponse(
            registeredUser,
            pair);
    }

    // =========================================================
    // LOGIN
    // =========================================================

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var user = await db.Users
                       .Include(x => x.UserRoles)
                       .ThenInclude(x => x.Role)
                       .ThenInclude(x => x.RolePermissions)
                       .ThenInclude(x => x.Permission)
                       .FirstOrDefaultAsync(
                           x => x.Email == email &&
                                !x.IsDeleted,
                           cancellationToken)
                   ?? throw new UnauthorizedException(
                       "Invalid email or password.");

        if (!user.IsActive)
        {
            throw new ForbiddenException(
                "This account is disabled.");
        }

        var verifyResult =
            passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (verifyResult ==
            PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        var pair =
            await jwtTokenService.CreateTokenPairAsync(
                user,
                cancellationToken);

        db.RefreshTokens.Add(
            new RefreshToken(
                user.Id,
                jwtTokenService.HashRefreshToken(
                    pair.RefreshToken),
                pair.RefreshTokenExpiresAtUtc));

        await db.SaveChangesAsync(cancellationToken);

        return BuildResponse(
            user,
            pair);
    }

    // =========================================================
    // REFRESH TOKEN
    // =========================================================

    public async Task<AuthResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            jwtTokenService.HashRefreshToken(
                request.RefreshToken);

        var stored = await db.RefreshTokens
                         .Include(x => x.User)
                         .ThenInclude(x => x.UserRoles)
                         .ThenInclude(x => x.Role)
                         .ThenInclude(x => x.RolePermissions)
                         .ThenInclude(x => x.Permission)
                         .FirstOrDefaultAsync(
                             x => x.TokenHash == tokenHash,
                             cancellationToken)
                     ?? throw new ForbiddenException(
                         "Refresh token is invalid.");

        if (!stored.IsActive ||
            !stored.User.IsActive)
        {
            throw new ForbiddenException(
                "Refresh token is expired or revoked.");
        }

        // Revoke old refresh token
        stored.Revoke();

        // Generate new pair
        var pair =
            await jwtTokenService.CreateTokenPairAsync(
                stored.User,
                cancellationToken);

        // Save new refresh token
        db.RefreshTokens.Add(
            new RefreshToken(
                stored.User.Id,
                jwtTokenService.HashRefreshToken(
                    pair.RefreshToken),
                pair.RefreshTokenExpiresAtUtc));

        await db.SaveChangesAsync(cancellationToken);

        return BuildResponse(
            stored.User,
            pair);
    }

    // =========================================================
    // LOGOUT
    // =========================================================

    public async Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var tokenHash =
            jwtTokenService.HashRefreshToken(
                refreshToken);

        var token = await db.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);

        // Logout should be idempotent.
        if (token is null)
        {
            return;
        }

        token.Revoke();

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // BUILD RESPONSE
    // =========================================================

    private static AuthResponse BuildResponse(
        User user,
        TokenPair pair)
    {
        var roles = user.UserRoles
            .Select(x => x.Role.Name)
            .Distinct()
            .Order()
            .ToList();

        var permissions = user.UserRoles
            .SelectMany(x => x.Role.RolePermissions)
            .Select(x => x.Permission.Name)
            .Distinct()
            .Order()
            .ToList();

        return new AuthResponse
        {
            AccessToken =
                pair.AccessToken,

            AccessTokenExpiresAtUtc =
                pair.AccessTokenExpiresAtUtc,

            RefreshToken =
                pair.RefreshToken,

            RefreshTokenExpiresAtUtc =
                pair.RefreshTokenExpiresAtUtc,

            User = new UserSummaryResponse
            {
                Id = user.Id,

                FullName =
                    user.FullName,

                Email =
                    user.Email,

                HotelId =
                    user.HotelId,

                BranchId =
                    user.BranchId,

                Roles =
                    roles,

                Permissions =
                    permissions
            }
        };
    }
}
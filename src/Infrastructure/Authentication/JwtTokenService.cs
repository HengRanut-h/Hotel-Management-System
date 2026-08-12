using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HotelManagement.Application.Abstractions.Authentication;
using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Domain.Modules.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HotelManagement.Infrastructure.Authentication;

public sealed class JwtTokenService(
    IOptions<JwtOptions> options,
    IApplicationDbContext db) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<TokenPair> CreateTokenPairAsync(
        User user,
        CancellationToken cancellationToken)
    {
        var roles = await db.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Role.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        var permissions = await db.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == user.Id)
            .SelectMany(x => x.Role.RolePermissions)
            .Select(x => x.Permission.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var accessExpires = now.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email)
        };

        if (user.HotelId.HasValue)
            claims.Add(new Claim("hotel_id", user.HotelId.Value.ToString()));

        if (user.BranchId.HasValue)
            claims.Add(new Claim("branch_id", user.BranchId.Value.ToString()));

        claims.AddRange(roles.Select(role =>
            new Claim(ClaimTypes.Role, role)));

        claims.AddRange(permissions.Select(permission =>
            new Claim("permission", permission)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: accessExpires.UtcDateTime,
            signingCredentials: credentials);

        var accessToken =
            new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken =
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var refreshExpires =
            now.AddDays(_options.RefreshTokenDays);

        return new TokenPair(
            accessToken,
            accessExpires,
            refreshToken,
            refreshExpires);
    }

    public string HashRefreshToken(string rawToken)
    {
        var bytes = Encoding.UTF8.GetBytes(rawToken);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}

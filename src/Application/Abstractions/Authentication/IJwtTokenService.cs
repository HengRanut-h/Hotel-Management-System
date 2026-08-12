using HotelManagement.Domain.Modules.Identity.Entities;

namespace HotelManagement.Application.Abstractions.Authentication;

public interface IJwtTokenService
{
    Task<TokenPair> CreateTokenPairAsync(User user, CancellationToken cancellationToken);
    string HashRefreshToken(string rawToken);
}

public sealed record TokenPair(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);
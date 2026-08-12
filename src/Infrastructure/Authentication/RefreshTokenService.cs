using System.Security.Cryptography;
using System.Text;
using HotelManagement.Application.Abstractions.Authentication;

namespace HotelManagement.Infrastructure.Authentication;

public sealed class RefreshTokenService : IRefreshTokenService
{
    public string Generate() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public string Hash(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
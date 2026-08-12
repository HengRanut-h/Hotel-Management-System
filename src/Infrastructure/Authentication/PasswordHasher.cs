using HotelManagement.Domain.Modules.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace HotelManagement.Infrastructure.Authentication;

public sealed class HotelPasswordHasher : IPasswordHasher<User>
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<User> _inner = new();
    public string HashPassword(User user, string password) => _inner.HashPassword(user, password);

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword) =>
        _inner.VerifyHashedPassword(user, hashedPassword, providedPassword);
}
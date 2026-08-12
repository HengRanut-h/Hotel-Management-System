namespace HotelManagement.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "HotelManagement";
    public string Audience { get; set; } = "HotelManagement.Web";
    public string Key { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 30;
    public int RefreshTokenDays { get; set; } = 7;
}

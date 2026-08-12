namespace HotelManagement.Api.RateLimiting;

public static class RateLimitPolicies
{
    public const string Auth = "auth";
    public const string Api = "api";
    public const string Write = "write";
    public const string Expensive = "expensive";
    public const string Upload = "upload";
}
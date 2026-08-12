namespace HotelManagement.Api.Extensions;

public static class HttpContextExtensions
{
    public static string CorrelationId(this HttpContext c) => c.TraceIdentifier;
}
namespace HotelManagement.Api.Middleware;

public sealed class HotelContextMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext c)
    {
        if (c.User.Identity?.IsAuthenticated == true && c.User.FindFirst("hotel_id") is { Value.Length: > 0 } h)
            c.Items["HotelId"] = h.Value;
        return next(c);
    }
}
namespace HotelManagement.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var id = context.Request.Headers.TryGetValue("X-Correlation-ID", out var v) && !string.IsNullOrWhiteSpace(v)
            ? v.ToString()
            : Guid.NewGuid().ToString("N");
        context.TraceIdentifier = id;
        context.Response.Headers["X-Correlation-ID"] = id;
        await next(context);
    }
}
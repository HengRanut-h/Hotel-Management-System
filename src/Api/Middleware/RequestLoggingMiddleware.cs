namespace HotelManagement.Api.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> log)
{
    public async Task InvokeAsync(HttpContext c)
    {
        var start = DateTimeOffset.UtcNow;
        await next(c);
        log.LogInformation("{Method} {Path} -> {StatusCode} in {Elapsed} ms", c.Request.Method, c.Request.Path,
            c.Response.StatusCode, (DateTimeOffset.UtcNow - start).TotalMilliseconds);
    }
}
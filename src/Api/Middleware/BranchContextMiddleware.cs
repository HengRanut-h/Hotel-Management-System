namespace HotelManagement.Api.Middleware;

public sealed class BranchContextMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext c)
    {
        if (c.User.Identity?.IsAuthenticated == true && c.User.FindFirst("branch_id") is { Value.Length: > 0 } b)
            c.Items["BranchId"] = b.Value;
        return next(c);
    }
}
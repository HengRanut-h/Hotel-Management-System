using System.Net;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Common.Exceptions;

namespace HotelManagement.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled request exception.");

            var (status, title) =
                ex switch
                {
                    UnauthorizedException =>
                        (HttpStatusCode.Unauthorized, ex.Message),

                    NotFoundException =>
                        (HttpStatusCode.NotFound, ex.Message),

                    ConflictException =>
                        (HttpStatusCode.Conflict, ex.Message),

                    ForbiddenException =>
                        (HttpStatusCode.Forbidden, ex.Message),

                    DomainException =>
                        (HttpStatusCode.UnprocessableEntity, ex.Message),

                    ArgumentException =>
                        (HttpStatusCode.BadRequest, ex.Message),

                    _ =>
                        (
                            HttpStatusCode.InternalServerError,
                            "An unexpected error occurred."
                        )
                };

            context.Response.StatusCode =
                (int)status;

            context.Response.ContentType =
                "application/problem+json";

            await context.Response.WriteAsJsonAsync(
                new
                {
                    type =
                        $"https://httpstatuses.com/{(int)status}",
                    title,
                    status = (int)status,
                    traceId =
                        context.TraceIdentifier
                });
        }
    }
}

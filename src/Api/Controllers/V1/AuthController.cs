using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Authentication.Contracts;
using HotelManagement.Application.Features.Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/auth")]
[EnableRateLimiting("auth")]
public sealed class AuthController(
    AuthService service) : ControllerBase
{
    // =========================================================
    // REGISTER
    // =========================================================

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.RegisterAsync(
            request,
            cancellationToken);

        return Created(
            string.Empty,
            ApiResponse<AuthResponse>.Created(
                response,
                "Account registered successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // LOGIN
    // =========================================================

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.LoginAsync(
            request,
            cancellationToken);

        return Ok(
            ApiResponse<AuthResponse>.Ok(
                response,
                "Login successful.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // REFRESH TOKEN
    // =========================================================

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.RefreshAsync(
            request,
            cancellationToken);

        return Ok(
            ApiResponse<AuthResponse>.Ok(
                response,
                "Token refreshed successfully.",
                code: "TOKEN_REFRESHED",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // LOGOUT
    // =========================================================

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        await service.LogoutAsync(
            request.RefreshToken,
            cancellationToken);

        return NoContent();
    }
}
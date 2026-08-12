using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Dashboard.Services;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/dashboard")]
public sealed class DashboardController(
    DashboardService service,
    ICurrentUser currentUser)
    : ControllerBase
{
    // =========================================================
    // GET DASHBOARD
    // =========================================================

    [HasPermission(Permissions.Dashboard.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> Get(
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.GetAsync(
                hotelId,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                response,
                "Dashboard retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }
}
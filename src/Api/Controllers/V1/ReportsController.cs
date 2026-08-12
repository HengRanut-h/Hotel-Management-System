using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Reports;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/reports")]
public sealed class ReportsController(
    ReportsService service)
    : ControllerBase
{
    // =========================================================
    // REVENUE REPORT
    // =========================================================

    [HasPermission("reports.view")]
    [HttpGet("revenue")]
    public async Task<ActionResult<ApiResponse<object>>> Revenue(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        var response =
            await service.RevenueAsync(
                from,
                to,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                response,
                "Revenue report retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // OCCUPANCY REPORT
    // =========================================================

    [HasPermission("reports.view")]
    [HttpGet("occupancy")]
    public async Task<ActionResult<ApiResponse<object>>> Occupancy(
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var response =
            await service.OccupancyAsync(
                date,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                response,
                "Occupancy report retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }
}
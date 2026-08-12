using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.MeterReadings;
using HotelManagement.Application.Features.Utilities.Contracts;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/meter-readings")]
public sealed class MeterReadingsController(
    MeterReadingsService service)
    : ControllerBase
{
    // =========================================================
    // GET BY METER
    // =========================================================

    [HasPermission(Permissions.Utilities.View)]
    [HttpGet("meter/{meterId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MeterReadingResponse>>>> Get(
        Guid meterId,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetByMeterAsync(
                meterId,
                cancellationToken);

        return Ok(
            ApiResponse<IEnumerable<MeterReadingResponse>>.Ok(
                response,
                "Meter readings retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // RECORD READING
    // =========================================================

    [HasPermission(Permissions.Utilities.RecordReading)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<MeterReadingResponse>>> Create(
        RecordMeterReadingRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.RecordAsync(
                request,
                cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<MeterReadingResponse>.Created(
                response,
                "Meter reading recorded successfully.",
                traceId: HttpContext.TraceIdentifier));
    }
}
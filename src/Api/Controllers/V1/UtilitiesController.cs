using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Utilities.Contracts;
using HotelManagement.Application.Features.Utilities.Services;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/utilities")]
public sealed class UtilitiesController(
    UtilityService service,
    ICurrentUser currentUser)
    : ControllerBase
{
    // =========================================================
    // GET METERS
    // GET /api/v1/utilities/meters
    // =========================================================

    [HasPermission(Permissions.Utilities.View)]
    [HttpGet("meters")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UtilityMeterResponse>>>> GetMeters(
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.GetMetersAsync(
                hotelId,
                cancellationToken);

        return Ok(
            ApiResponse<IEnumerable<UtilityMeterResponse>>.Ok(
                response,
                "Utility meters retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET METER BY ID
    // GET /api/v1/utilities/meters/{id}
    // =========================================================

    [HasPermission(Permissions.Utilities.View)]
    [HttpGet("meters/{id:guid}")]
    public async Task<ActionResult<ApiResponse<UtilityMeterResponse>>> GetMeterById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.GetMeterByIdAsync(
                hotelId,
                id,
                cancellationToken);

        return Ok(
            ApiResponse<UtilityMeterResponse>.Ok(
                response,
                "Utility meter retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE METER
    // POST /api/v1/utilities/meters
    // =========================================================

    [HasPermission(Permissions.Utilities.ManageRate)]
    [HttpPost("meters")]
    public async Task<ActionResult<ApiResponse<UtilityMeterResponse>>> CreateMeter(
        [FromBody] CreateUtilityMeterRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.CreateMeterAsync(
                hotelId,
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetMeterById),
            new
            {
                id = response.Id
            },
            ApiResponse<UtilityMeterResponse>.Created(
                response,
                "Utility meter created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // RECORD READING
    // POST /api/v1/utilities/readings
    // =========================================================

    [HasPermission(Permissions.Utilities.RecordReading)]
    [HttpPost("readings")]
    public async Task<ActionResult<ApiResponse<MeterReadingResponse>>> RecordReading(
        [FromBody] RecordMeterReadingRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.RecordReadingAsync(
                hotelId,
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

using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Utilities.Contracts;
using HotelManagement.Application.Features.UtilityMeters;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/utility-meters")]
public sealed class UtilityMetersController(
    UtilityMetersService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // GET /api/v1/utility-meters
    // =========================================================

    [HasPermission(Permissions.Utilities.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UtilityMeterResponse>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAsync(
                cancellationToken);

        return Ok(
            ApiResponse<IEnumerable<UtilityMeterResponse>>.Ok(
                response,
                "Utility meters retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // GET /api/v1/utility-meters/{id}
    // =========================================================

    [HasPermission(Permissions.Utilities.View)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UtilityMeterResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetByIdAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<UtilityMeterResponse>.Ok(
                response,
                "Utility meter retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // POST /api/v1/utility-meters
    // =========================================================

    [HasPermission(Permissions.Utilities.ManageRate)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UtilityMeterResponse>>> Create(
        [FromBody] CreateUtilityMeterRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = response.Id
            },
            ApiResponse<UtilityMeterResponse>.Created(
                response,
                "Utility meter created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }
}
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Housekeeping.Contracts;
using HotelManagement.Application.Features.Housekeeping.Services;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/housekeeping")]
public sealed class HousekeepingController(
    HousekeepingService service,
    ICurrentUser currentUser)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission(Permissions.Housekeeping.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.GetAllAsync(
                hotelId,
                cancellationToken);

        return Ok(
            ApiResponse<IEnumerable<object>>.Ok(
                response,
                "Housekeeping tasks retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission(Permissions.Housekeeping.View)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.GetByIdAsync(
                hotelId,
                id,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                response,
                "Housekeeping task retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission(Permissions.Housekeeping.Manage)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        CreateHousekeepingTaskRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var id =
            await service.CreateAsync(
                hotelId,
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id
            },
            ApiResponse<Guid>.Created(
                id,
                "Housekeeping task created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // COMPLETE
    // =========================================================

    [HasPermission(Permissions.Housekeeping.Manage)]
    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<ApiResponse<object?>>> Complete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        await service.CompleteAsync(
            hotelId,
            id,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Housekeeping task completed successfully.",
                "HOUSEKEEPING_COMPLETED",
                HttpContext.TraceIdentifier));
    }
}
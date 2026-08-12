using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Maintenance.Contracts;
using HotelManagement.Application.Features.Maintenance.Services;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/maintenance")]
public sealed class MaintenanceController(
    MaintenanceService service,
    ICurrentUser currentUser)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission(Permissions.Maintenance.View)]
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
                "Maintenance requests retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission(Permissions.Maintenance.View)]
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
                "Maintenance request retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission(Permissions.Maintenance.Manage)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        CreateMaintenanceRequest request,
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
                "Maintenance request created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // COMPLETE
    // =========================================================

    [HasPermission(Permissions.Maintenance.Manage)]
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
                "Maintenance request completed successfully.",
                "MAINTENANCE_COMPLETED",
                HttpContext.TraceIdentifier));
    }
}
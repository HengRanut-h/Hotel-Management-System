using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.RoomAssignments;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/room-assignments")]
public sealed class RoomAssignmentsController(
    RoomAssignmentsService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission("room-assignments.view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<OperationalResponse>>>> GetAll(
        [FromQuery] OperationalQuery request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAllAsync(
                request,
                cancellationToken);

        var totalPages =
            response.PageSize == 0
                ? 0
                : (int)Math.Ceiling(
                    response.TotalItems /
                    (double)response.PageSize);

        return Ok(
            ApiResponse<IEnumerable<OperationalResponse>>.Ok(
                response.Items,
                "Room assignments retrieved successfully.",
                meta: new
                {
                    pagination = new
                    {
                        response.PageNumber,
                        response.PageSize,
                        response.TotalItems,
                        totalPages,
                        hasPreviousPage =
                            response.PageNumber > 1,
                        hasNextPage =
                            response.PageNumber < totalPages
                    }
                },
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission("room-assignments.view")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OperationalResponse>>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetByIdAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<OperationalResponse>.Ok(
                response,
                "Room assignment retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission("room-assignments.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OperationalResponse>>> Create(
        OperationalCreateRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(Get),
            new
            {
                id = response.Id
            },
            ApiResponse<OperationalResponse>.Created(
                response,
                "Room assignment created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission("room-assignments.update")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OperationalResponse>>> Update(
        Guid id,
        OperationalUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.UpdateAsync(
                id,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<OperationalResponse>.Updated(
                response,
                "Room assignment updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CHANGE STATUS
    // =========================================================

    [HasPermission("room-assignments.manage")]
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<object?>>> Status(
        Guid id,
        ChangeStatusRequest request,
        CancellationToken cancellationToken)
    {
        await service.ChangeStatusAsync(
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Room assignment status updated successfully.",
                "STATUS_CHANGED",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HasPermission("room-assignments.delete")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}
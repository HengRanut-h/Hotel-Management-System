using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.RoomTypes;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/room-types")]
public sealed class RoomTypesController(
    RoomTypesService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // GET /api/v1/room-types
    // =========================================================

    [HasPermission("room-types.view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RoomTypeResponse>>>> GetAll(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var response =
            await service.GetAllAsync(
                pageNumber,
                pageSize,
                search,
                cancellationToken);

        var totalPages =
            response.PageSize == 0
                ? 0
                : (int)Math.Ceiling(
                    response.TotalItems /
                    (double)response.PageSize);

        return Ok(
            ApiResponse<IEnumerable<RoomTypeResponse>>.Ok(
                response.Items,
                "Room types retrieved successfully.",
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
    // GET /api/v1/room-types/{id}
    // =========================================================

    [HasPermission("room-types.view")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<RoomTypeResponse>.Ok(
                response,
                "Room type retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // POST /api/v1/room-types
    // =========================================================

    [HasPermission("room-types.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> Create(
        [FromBody] RoomTypeRequest request,
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
            ApiResponse<RoomTypeResponse>.Created(
                response,
                "Room type created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // PUT /api/v1/room-types/{id}
    // =========================================================

    [HasPermission("room-types.update")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> Update(
        Guid id,
        [FromBody] RoomTypeRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.UpdateAsync(
                id,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<RoomTypeResponse>.Updated(
                response,
                "Room type updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // PATCH /api/v1/room-types/{id}/active
    // =========================================================

    [HasPermission("room-types.update")]
    [HttpPatch("{id:guid}/active")]
    public async Task<ActionResult<ApiResponse<RoomTypeResponse>>> SetActive(
        Guid id,
        [FromBody] CatalogActiveRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.SetActiveAsync(
                id,
                request.IsActive,
                cancellationToken);

        return Ok(
            request.IsActive
                ? ApiResponse<RoomTypeResponse>.Activated(
                    response,
                    "Room type activated successfully.",
                    HttpContext.TraceIdentifier)
                : ApiResponse<RoomTypeResponse>.Deactivated(
                    response,
                    "Room type deactivated successfully.",
                    HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // DELETE /api/v1/room-types/{id}
    // =========================================================

    [HasPermission("room-types.delete")]
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
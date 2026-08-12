using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Transportation;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/transportation")]
public sealed class TransportationController(
    TransportationService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // GET /api/v1/transportation
    // =========================================================

    [HasPermission("transportation.view")]
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
                "Transportation requests retrieved successfully.",
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
    // GET /api/v1/transportation/{id}
    // =========================================================

    [HasPermission("transportation.view")]
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
                "Transportation request retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // POST /api/v1/transportation
    // =========================================================

    [HasPermission("transportation.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OperationalResponse>>> Create(
        [FromBody] OperationalCreateRequest request,
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
                "Transportation request created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // PUT /api/v1/transportation/{id}
    // =========================================================

    [HasPermission("transportation.update")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OperationalResponse>>> Update(
        Guid id,
        [FromBody] OperationalUpdateRequest request,
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
                "Transportation request updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CHANGE STATUS
    // PATCH /api/v1/transportation/{id}/status
    // =========================================================

    [HasPermission("transportation.manage")]
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<object?>>> Status(
        Guid id,
        [FromBody] ChangeStatusRequest request,
        CancellationToken cancellationToken)
    {
        await service.ChangeStatusAsync(
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Transportation request status updated successfully.",
                "STATUS_CHANGED",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // DELETE /api/v1/transportation/{id}
    // =========================================================

    [HasPermission("transportation.delete")]
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
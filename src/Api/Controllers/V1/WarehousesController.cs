using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Warehouses;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/warehouses")]
public sealed class WarehousesController(
    WarehousesService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // GET /api/v1/warehouses
    // =========================================================

    [HasPermission("warehouses.view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CatalogResponse>>>> GetAll(
        [FromQuery] CatalogQuery request,
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
            ApiResponse<IEnumerable<CatalogResponse>>.Ok(
                response.Items,
                "Warehouses retrieved successfully.",
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
    // GET /api/v1/warehouses/{id}
    // =========================================================

    [HasPermission("warehouses.view")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetByIdAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<CatalogResponse>.Ok(
                response,
                "Warehouse retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // POST /api/v1/warehouses
    // =========================================================

    [HasPermission("warehouses.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> Create(
        [FromBody] CatalogCreateRequest request,
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
            ApiResponse<CatalogResponse>.Created(
                response,
                "Warehouse created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // PUT /api/v1/warehouses/{id}
    // =========================================================

    [HasPermission("warehouses.update")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> Update(
        Guid id,
        [FromBody] CatalogUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.UpdateAsync(
                id,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<CatalogResponse>.Updated(
                response,
                "Warehouse updated successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // PATCH /api/v1/warehouses/{id}/active
    // =========================================================

    [HasPermission("warehouses.update")]
    [HttpPatch("{id:guid}/active")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> SetActive(
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
                ? ApiResponse<CatalogResponse>.Activated(
                    response,
                    "Warehouse activated successfully.",
                    HttpContext.TraceIdentifier)
                : ApiResponse<CatalogResponse>.Deactivated(
                    response,
                    "Warehouse deactivated successfully.",
                    HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // DELETE /api/v1/warehouses/{id}
    // =========================================================

    [HasPermission("warehouses.delete")]
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

    // =========================================================
    // RESTORE
    // POST /api/v1/warehouses/{id}/restore
    // =========================================================

    [HasPermission("warehouses.delete")]
    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> Restore(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.RestoreAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<CatalogResponse>.Restored(
                response,
                "Warehouse restored successfully.",
                HttpContext.TraceIdentifier));
    }
}
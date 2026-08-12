using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Buildings;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/buildings")]
public sealed class BuildingsController(
    BuildingsService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission("buildings.view")]
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
                "Buildings retrieved successfully.",
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

    [HasPermission("buildings.view")]
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
                "Building retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission("buildings.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> Create(
        CatalogCreateRequest request,
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
                "Building created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission("buildings.update")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> Update(
        Guid id,
        CatalogUpdateRequest request,
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
                "Building updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    [HasPermission("buildings.update")]
    [HttpPatch("{id:guid}/active")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> SetActive(
        Guid id,
        CatalogActiveRequest request,
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
                    "Building activated successfully.",
                    HttpContext.TraceIdentifier)
                : ApiResponse<CatalogResponse>.Deactivated(
                    response,
                    "Building deactivated successfully.",
                    HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HasPermission("buildings.delete")]
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
    // =========================================================

    [HasPermission("buildings.delete")]
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
                "Building restored successfully.",
                HttpContext.TraceIdentifier));
    }
}
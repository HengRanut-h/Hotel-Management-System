using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.FeatureFlags;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/feature-flags")]
public sealed class FeatureFlagsController(
    FeatureFlagsService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission("feature-flags.view")]
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
                "Feature flags retrieved successfully.",
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

    [HasPermission("feature-flags.view")]
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
                "Feature flag retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission("feature-flags.create")]
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
                "Feature flag created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission("feature-flags.update")]
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
                "Feature flag updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    [HasPermission("feature-flags.update")]
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
                    "Feature flag activated successfully.",
                    HttpContext.TraceIdentifier)
                : ApiResponse<CatalogResponse>.Deactivated(
                    response,
                    "Feature flag deactivated successfully.",
                    HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HasPermission("feature-flags.delete")]
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

    [HasPermission("feature-flags.delete")]
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
                "Feature flag restored successfully.",
                HttpContext.TraceIdentifier));
    }
}
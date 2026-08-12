using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Taxes;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/taxes")]
public sealed class TaxesController(
    TaxesService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // GET /api/v1/taxes
    // =========================================================

    [HasPermission("taxes.view")]
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
                "Taxes retrieved successfully.",
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
    // GET /api/v1/taxes/{id}
    // =========================================================

    [HasPermission("taxes.view")]
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
                "Tax retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // POST /api/v1/taxes
    // =========================================================

    [HasPermission("taxes.create")]
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
                "Tax created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // PUT /api/v1/taxes/{id}
    // =========================================================

    [HasPermission("taxes.update")]
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
                "Tax updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // PATCH /api/v1/taxes/{id}/active
    // =========================================================

    [HasPermission("taxes.update")]
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
                    "Tax activated successfully.",
                    HttpContext.TraceIdentifier)
                : ApiResponse<CatalogResponse>.Deactivated(
                    response,
                    "Tax deactivated successfully.",
                    HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // DELETE /api/v1/taxes/{id}
    // =========================================================

    [HasPermission("taxes.delete")]
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
    // POST /api/v1/taxes/{id}/restore
    // =========================================================

    [HasPermission("taxes.delete")]
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
                "Tax restored successfully.",
                HttpContext.TraceIdentifier));
    }
}
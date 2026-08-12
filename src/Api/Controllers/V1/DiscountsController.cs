using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Discounts;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/discounts")]
public sealed class DiscountsController(
    DiscountsService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission("discounts.view")]
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
                "Discounts retrieved successfully.",
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

    [HasPermission("discounts.view")]
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
                "Discount retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission("discounts.create")]
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
                "Discount created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission("discounts.update")]
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
                "Discount updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    [HasPermission("discounts.update")]
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
                    "Discount activated successfully.",
                    HttpContext.TraceIdentifier)
                : ApiResponse<CatalogResponse>.Deactivated(
                    response,
                    "Discount deactivated successfully.",
                    HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HasPermission("discounts.delete")]
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

    [HasPermission("discounts.delete")]
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
                "Discount restored successfully.",
                HttpContext.TraceIdentifier));
    }
}
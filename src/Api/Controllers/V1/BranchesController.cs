using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Branches;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/branches")]
public sealed class BranchesController(
    BranchesService service) : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission("branches.view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CatalogResponse>>>> GetAll(
        [FromQuery] CatalogQuery request,
        CancellationToken cancellationToken)
    {
        var response = await service.GetAllAsync(
            request,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(
            response.TotalItems / (double)response.PageSize);

        var meta = new
        {
            pagination = new
            {
                response.PageNumber,
                response.PageSize,
                response.TotalItems,
                TotalPages = totalPages,
                HasPreviousPage = response.PageNumber > 1,
                HasNextPage = response.PageNumber < totalPages
            }
        };

        return Ok(
            ApiResponse<IReadOnlyList<CatalogResponse>>.Ok(
                response.Items.ToList(),
                "Branches retrieved successfully.",
                meta: meta,
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission("branches.view")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await service.GetAsync(
            id,
            cancellationToken);

        return Ok(
            ApiResponse<CatalogResponse>.Ok(
                response,
                "Branch retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission("branches.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> Create(
        [FromBody] CatalogCreateRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.CreateAsync(
            request,
            cancellationToken);

        var apiResponse =
            ApiResponse<CatalogResponse>.Created(
                response,
                "Branch created successfully.",
                traceId: HttpContext.TraceIdentifier);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = response.Id
            },
            apiResponse);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission("branches.update")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> Update(
        Guid id,
        [FromBody] CatalogUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.UpdateAsync(
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<CatalogResponse>.Updated(
                response,
                "Branch updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    [HasPermission("branches.update")]
    [HttpPatch("{id:guid}/active")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> SetActive(
        Guid id,
        [FromBody] CatalogActiveRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.SetActiveAsync(
            id,
            request.IsActive,
            cancellationToken);

        if (request.IsActive)
        {
            return Ok(
                ApiResponse<CatalogResponse>.Activated(
                    response,
                    "Branch activated successfully.",
                    HttpContext.TraceIdentifier));
        }

        return Ok(
            ApiResponse<CatalogResponse>.Deactivated(
                response,
                "Branch deactivated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // SOFT DELETE
    // =========================================================

    [HasPermission("branches.delete")]
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

    [HasPermission("branches.delete")]
    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> Restore(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await service.RestoreAsync(
            id,
            cancellationToken);

        return Ok(
            ApiResponse<CatalogResponse>.Restored(
                response,
                "Branch restored successfully.",
                HttpContext.TraceIdentifier));
    }
}
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Amenities;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/amenities")]
public sealed class AmenitiesController(
    AmenitiesService service) : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HttpGet]
    [HasPermission("amenities.view")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CatalogResponse>>>> GetAll(
        [FromQuery] CatalogQuery query,
        CancellationToken cancellationToken)
    {
        var response = await service.GetAllAsync(
            query,
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
                "Amenities retrieved successfully.",
                meta: meta,
                traceId: HttpContext.TraceIdentifier));
    }


    // =========================================================
    // GET BY ID
    // =========================================================

    [HttpGet("{id:guid}")]
    [HasPermission("amenities.view")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await service.GetByIdAsync(
            id,
            cancellationToken);

        return Ok(
            ApiResponse<CatalogResponse>.Ok(
                response,
                "Amenity retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }


    // =========================================================
    // CREATE
    // =========================================================

    [HttpPost]
    [HasPermission("amenities.create")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> Create(
        [FromBody] CatalogCreateRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.CreateAsync(
            request,
            cancellationToken);

        var apiResponse = ApiResponse<CatalogResponse>.Created(
            response,
            "Amenity created successfully.",
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

    [HttpPut("{id:guid}")]
    [HasPermission("amenities.update")]
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
                "Amenity updated successfully.",
                traceId: HttpContext.TraceIdentifier));
    }


    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    [HttpPatch("{id:guid}/active")]
    [HasPermission("amenities.update")]
    public async Task<ActionResult<ApiResponse<CatalogResponse>>> SetActive(
        Guid id,
        [FromBody] CatalogActiveRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.SetActiveAsync(
            id,
            request.IsActive,
            cancellationToken);

        var apiResponse = request.IsActive
            ? ApiResponse<CatalogResponse>.Activated(
                response,
                "Amenity activated successfully.",
                HttpContext.TraceIdentifier)
            : ApiResponse<CatalogResponse>.Deactivated(
                response,
                "Amenity deactivated successfully.",
                HttpContext.TraceIdentifier);

        return Ok(apiResponse);
    }


    // =========================================================
    // SOFT DELETE
    // =========================================================

    [HttpDelete("{id:guid}")]
    [HasPermission("amenities.delete")]
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

    [HttpPost("{id:guid}/restore")]
    [HasPermission("amenities.delete")]
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
                "Amenity restored successfully.",
                traceId: HttpContext.TraceIdentifier));
    }
}
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Hotels;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/hotels")]
public sealed class HotelsController(
    HotelsService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission("hotels.view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<HotelResponse>>>> GetAll(
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
            ApiResponse<IEnumerable<HotelResponse>>.Ok(
                response.Items,
                "Hotels retrieved successfully.",
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

    [HasPermission("hotels.view")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<HotelResponse>>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<HotelResponse>.Ok(
                response,
                "Hotel retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission("hotels.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<HotelResponse>>> Create(
        HotelRequest request,
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
            ApiResponse<HotelResponse>.Created(
                response,
                "Hotel created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission("hotels.update")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<HotelResponse>>> Update(
        Guid id,
        HotelRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.UpdateAsync(
                id,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<HotelResponse>.Updated(
                response,
                "Hotel updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    [HasPermission("hotels.update")]
    [HttpPatch("{id:guid}/active")]
    public async Task<ActionResult<ApiResponse<HotelResponse>>> SetActive(
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
                ? ApiResponse<HotelResponse>.Activated(
                    response,
                    "Hotel activated successfully.",
                    HttpContext.TraceIdentifier)
                : ApiResponse<HotelResponse>.Deactivated(
                    response,
                    "Hotel deactivated successfully.",
                    HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HasPermission("hotels.delete")]
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
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Inventory;
using HotelManagement.Domain.Modules.Inventory.Entities;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/inventory")]
public sealed class InventoryController(
    InventoryService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission("inventory.view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<InventoryItem>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAsync(
                cancellationToken);

        return Ok(
            ApiResponse<IEnumerable<InventoryItem>>.Ok(
                response,
                "Inventory items retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission("inventory.view")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<InventoryItem>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetByIdAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<InventoryItem>.Ok(
                response,
                "Inventory item retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission("inventory.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<InventoryItem>>> Create(
        InventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = response.Id
            },
            ApiResponse<InventoryItem>.Created(
                response,
                "Inventory item created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ADJUST STOCK
    // =========================================================

    [HasPermission("inventory.manage")]
    [HttpPost("{id:guid}/adjust")]
    public async Task<ActionResult<ApiResponse<object?>>> Adjust(
        Guid id,
        StockAdjustRequest request,
        CancellationToken cancellationToken)
    {
        await service.AdjustAsync(
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Inventory stock adjusted successfully.",
                "STOCK_ADJUSTED",
                HttpContext.TraceIdentifier));
    }
}
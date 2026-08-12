using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Restaurant;
using HotelManagement.Domain.Modules.Restaurant.Entities;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/restaurant")]
public sealed class RestaurantController(
    RestaurantService service)
    : ControllerBase
{
    // =========================================================
    // GET MENU
    // =========================================================

    [HasPermission("restaurant.view")]
    [HttpGet("menu")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuItem>>>> Menu(
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetMenuAsync(
                cancellationToken);

        return Ok(
            ApiResponse<IEnumerable<MenuItem>>.Ok(
                response,
                "Restaurant menu retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE CATEGORY
    // =========================================================

    [HasPermission("restaurant.manage")]
    [HttpPost("categories")]
    public async Task<ActionResult<ApiResponse<MenuCategory>>> CreateCategory(
        MenuCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.CreateCategoryAsync(
                request,
                cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<MenuCategory>.Created(
                response,
                "Menu category created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE MENU ITEM
    // =========================================================

    [HasPermission("restaurant.manage")]
    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<MenuItem>>> CreateItem(
        MenuItemRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.CreateItemAsync(
                request,
                cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<MenuItem>.Created(
                response,
                "Menu item created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE ORDER
    // =========================================================

    [HasPermission("restaurant.orders")]
    [HttpPost("orders")]
    public async Task<ActionResult<ApiResponse<RestaurantOrder>>> CreateOrder(
        CreateRestaurantOrderRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.CreateOrderAsync(
                request,
                cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<RestaurantOrder>.Created(
                response,
                "Restaurant order created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ADD ORDER ITEM
    // =========================================================

    [HasPermission("restaurant.orders")]
    [HttpPost("orders/{id:guid}/items")]
    public async Task<ActionResult<ApiResponse<object?>>> AddItem(
        Guid id,
        AddRestaurantOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        await service.AddItemAsync(
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Restaurant order item added successfully.",
                "ORDER_ITEM_ADDED",
                HttpContext.TraceIdentifier));
    }
}
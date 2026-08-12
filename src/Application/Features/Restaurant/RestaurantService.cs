using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Modules.Restaurant.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Restaurant;

public sealed record MenuCategoryRequest(
    string Name);

public sealed record MenuItemRequest(
    Guid CategoryId,
    string Name,
    decimal Price);

public sealed record CreateRestaurantOrderRequest(
    Guid? RoomId,
    Guid? GuestId,
    Guid? BranchId);

public sealed record AddRestaurantOrderItemRequest(
    Guid MenuItemId,
    int Quantity);

public sealed class RestaurantService(
    IApplicationDbContext db,
    ICurrentUser currentUser)
{
    // =========================================================
    // HOTEL CONTEXT
    // =========================================================

    private Guid HotelId =>
        currentUser.HotelId
        ?? throw new ForbiddenException(
            "Hotel context required.");

    // =========================================================
    // GET MENU
    // =========================================================

    public Task<List<MenuItem>> GetMenuAsync(
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        return db.MenuItems
            .AsNoTracking()
            .Where(
                menuItem =>
                    menuItem.HotelId == hotelId
                    &&
                    !menuItem.IsDeleted
                    &&
                    menuItem.IsAvailable)
            .OrderBy(
                menuItem =>
                    menuItem.Name)
            .ToListAsync(
                cancellationToken);
    }

    // =========================================================
    // CREATE CATEGORY
    // =========================================================

    public async Task<MenuCategory> CreateCategoryAsync(
        MenuCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ConflictException(
                "Category name is required.");
        }

        var category =
            new MenuCategory(
                hotelId,
                request.Name.Trim());

        db.MenuCategories.Add(
            category);

        await db.SaveChangesAsync(
            cancellationToken);

        return category;
    }

    // =========================================================
    // CREATE MENU ITEM
    // =========================================================

    public async Task<MenuItem> CreateItemAsync(
        MenuItemRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        if (request.Price <= 0)
        {
            throw new ConflictException(
                "Menu item price must be greater than zero.");
        }

        var categoryExists =
            await db.MenuCategories.AnyAsync(
                category =>
                    category.Id == request.CategoryId
                    &&
                    category.HotelId == hotelId
                    &&
                    !category.IsDeleted,
                cancellationToken);

        if (!categoryExists)
        {
            throw new NotFoundException(
                "Menu category not found.");
        }

        var menuItem =
            new MenuItem(
                hotelId,
                request.CategoryId,
                request.Name.Trim(),
                request.Price);

        db.MenuItems.Add(
            menuItem);

        await db.SaveChangesAsync(
            cancellationToken);

        return menuItem;
    }

    // =========================================================
    // CREATE ORDER
    // =========================================================

    public async Task<RestaurantOrder> CreateOrderAsync(
        CreateRestaurantOrderRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var branchId =
            request.BranchId
            ?? currentUser.BranchId
            ?? throw new ConflictException(
                "Branch required.");

        var branchExists =
            await db.HotelBranches.AnyAsync(
                branch =>
                    branch.Id == branchId
                    &&
                    branch.HotelId == hotelId
                    &&
                    !branch.IsDeleted,
                cancellationToken);

        if (!branchExists)
        {
            throw new NotFoundException(
                "Branch not found.");
        }

        if (request.RoomId.HasValue)
        {
            var roomExists =
                await db.Rooms.AnyAsync(
                    room =>
                        room.Id == request.RoomId.Value
                        &&
                        room.HotelId == hotelId
                        &&
                        room.BranchId == branchId
                        &&
                        !room.IsDeleted,
                    cancellationToken);

            if (!roomExists)
            {
                throw new NotFoundException(
                    "Room not found.");
            }
        }

        if (request.GuestId.HasValue)
        {
            var guestExists =
                await db.Guests.AnyAsync(
                    guest =>
                        guest.Id == request.GuestId.Value
                        &&
                        guest.HotelId == hotelId
                        &&
                        !guest.IsDeleted,
                    cancellationToken);

            if (!guestExists)
            {
                throw new NotFoundException(
                    "Guest not found.");
            }
        }

        var orderNumber =
            $"ORD-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

        var order =
            new RestaurantOrder(
                hotelId,
                branchId,
                orderNumber,
                request.RoomId,
                request.GuestId);

        db.RestaurantOrders.Add(
            order);

        await db.SaveChangesAsync(
            cancellationToken);

        return order;
    }

    // =========================================================
    // ADD ORDER ITEM
    // =========================================================

    public async Task AddItemAsync(
        Guid orderId,
        AddRestaurantOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        if (request.Quantity <= 0)
        {
            throw new ConflictException(
                "Quantity must be greater than zero.");
        }

        var order =
            await db.RestaurantOrders
                .FirstOrDefaultAsync(
                    order =>
                        order.HotelId == hotelId
                        &&
                        order.Id == orderId
                        &&
                        !order.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Order not found.");

        var menuItem =
            await db.MenuItems
                .FirstOrDefaultAsync(
                    menuItem =>
                        menuItem.HotelId == hotelId
                        &&
                        menuItem.Id == request.MenuItemId
                        &&
                        !menuItem.IsDeleted
                        &&
                        menuItem.IsAvailable,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Menu item not found or unavailable.");

        var orderItem =
            new RestaurantOrderItem(
                order.Id,
                menuItem.Id,
                request.Quantity,
                menuItem.Price);

        db.RestaurantOrderItems.Add(
            orderItem);

        await db.SaveChangesAsync(
            cancellationToken);
    }
}
using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Modules.Inventory.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Inventory;

public sealed record InventoryItemRequest(
    string Sku,
    string Name,
    string Unit,
    decimal ReorderLevel);

public sealed record StockAdjustRequest(
    decimal Quantity,
    string Reason);

public sealed class InventoryService(
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
    // GET ALL
    // =========================================================

    public Task<List<InventoryItem>> GetAsync(
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        return db.InventoryItems
            .AsNoTracking()
            .Where(
                inventoryItem =>
                    inventoryItem.HotelId == hotelId
                    &&
                    !inventoryItem.IsDeleted)
            .OrderBy(
                inventoryItem =>
                    inventoryItem.Name)
            .ToListAsync(
                cancellationToken);
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<InventoryItem> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        return await db.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(
                inventoryItem =>
                    inventoryItem.HotelId == hotelId
                    &&
                    inventoryItem.Id == id
                    &&
                    !inventoryItem.IsDeleted,
                cancellationToken)
            ?? throw new NotFoundException(
                "Inventory item not found.");
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<InventoryItem> CreateAsync(
        InventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var sku =
            request.Sku
                .Trim()
                .ToUpperInvariant();

        var skuExists =
            await db.InventoryItems.AnyAsync(
                inventoryItem =>
                    inventoryItem.HotelId == hotelId
                    &&
                    inventoryItem.Sku == sku
                    &&
                    !inventoryItem.IsDeleted,
                cancellationToken);

        if (skuExists)
        {
            throw new ConflictException(
                "SKU already exists.");
        }

        var inventoryItem =
            new InventoryItem(
                hotelId,
                sku,
                request.Name.Trim(),
                request.Unit.Trim(),
                request.ReorderLevel);

        db.InventoryItems.Add(
            inventoryItem);

        await db.SaveChangesAsync(
            cancellationToken);

        return inventoryItem;
    }

    // =========================================================
    // ADJUST STOCK
    // =========================================================

    public async Task AdjustAsync(
        Guid id,
        StockAdjustRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var inventoryItem =
            await db.InventoryItems
                .FirstOrDefaultAsync(
                    inventoryItem =>
                        inventoryItem.HotelId == hotelId
                        &&
                        inventoryItem.Id == id
                        &&
                        !inventoryItem.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Inventory item not found.");

        inventoryItem.Adjust(
            request.Quantity);

        var referenceNumber =
            $"STK-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

        var stockTransaction =
            new StockTransaction(
                hotelId,
                currentUser.BranchId,
                referenceNumber,
                request.Reason,
                "Posted",
                request.Reason,
                0,
                DateTimeOffset.UtcNow,
                id,
                "InventoryItem");

        db.StockTransactions.Add(
            stockTransaction);

        await db.SaveChangesAsync(
            cancellationToken);
    }
}
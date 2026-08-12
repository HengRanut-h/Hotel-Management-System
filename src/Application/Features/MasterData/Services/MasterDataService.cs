using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.MasterData.Services;

public sealed class MasterDataService(
    IApplicationDbContext db)
{
    // =========================================================
    // GET MASTER DATA
    // =========================================================

    public async Task<object> GetAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // HOTEL
        // =====================================================

        var hotel =
            await db.Hotels
                .AsNoTracking()
                .Where(
                    hotel =>
                        hotel.Id == hotelId
                        &&
                        !hotel.IsDeleted)
                .Select(
                    hotel =>
                        new
                        {
                            hotel.Id,
                            hotel.Name,
                            hotel.Code,
                            hotel.Currency
                        })
                .FirstOrDefaultAsync(
                    cancellationToken)
            ?? throw new NotFoundException(
                "Hotel not found.");

        // =====================================================
        // BRANCHES
        // =====================================================

        var branches =
            await db.HotelBranches
                .AsNoTracking()
                .Where(
                    branch =>
                        branch.HotelId == hotelId
                        &&
                        !branch.IsDeleted
                        &&
                        branch.IsActive)
                .OrderBy(
                    branch =>
                        branch.Name)
                .Select(
                    branch =>
                        new
                        {
                            branch.Id,
                            branch.Name,
                            branch.Code
                        })
                .ToListAsync(
                    cancellationToken);

        // =====================================================
        // ROOM TYPES
        // =====================================================

        var roomTypes =
            await db.RoomTypes
                .AsNoTracking()
                .Where(
                    roomType =>
                        roomType.HotelId == hotelId
                        &&
                        !roomType.IsDeleted
                        &&
                        roomType.IsActive)
                .OrderBy(
                    roomType =>
                        roomType.Name)
                .Select(
                    roomType =>
                        new
                        {
                            roomType.Id,
                            roomType.Name,
                            roomType.Code,
                            roomType.BaseRate,
                            roomType.MaxAdults,
                            roomType.MaxChildren
                        })
                .ToListAsync(
                    cancellationToken);

        // =====================================================
        // RESPONSE
        // =====================================================

        return new
        {
            hotel,
            branches,
            roomTypes
        };
    }
}
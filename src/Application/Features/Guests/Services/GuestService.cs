using AutoMapper;
using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Features.Guests.Contracts;
using HotelManagement.Domain.Modules.Guests.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Guests.Services;

public sealed class GuestService(
    IApplicationDbContext db,
    IMapper mapper)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<PagedResult<GuestResponse>> GetAllAsync(
        Guid hotelId,
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        pageNumber =
            Math.Max(
                1,
                pageNumber);

        pageSize =
            Math.Clamp(
                pageSize,
                1,
                100);

        var query =
            db.Guests
                .AsNoTracking()
                .Where(
                    guest =>
                        guest.HotelId == hotelId
                        &&
                        !guest.IsDeleted);

        // =====================================================
        // SEARCH
        // =====================================================

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm =
                search.Trim();

            query =
                query.Where(
                    guest =>
                        guest.FirstName.Contains(searchTerm)
                        ||
                        guest.LastName.Contains(searchTerm)
                        ||
                        (
                            guest.Phone != null
                            &&
                            guest.Phone.Contains(searchTerm)
                        )
                        ||
                        (
                            guest.Email != null
                            &&
                            guest.Email.Contains(searchTerm)
                        ));
        }

        // =====================================================
        // TOTAL
        // =====================================================

        var totalItems =
            await query.CountAsync(
                cancellationToken);

        // =====================================================
        // PAGINATION
        // =====================================================

        var entities =
            await query
                .OrderBy(
                    guest =>
                        guest.FirstName)
                .ThenBy(
                    guest =>
                        guest.LastName)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(
                    pageSize)
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<GuestResponse>
        {
            Items =
                mapper.Map<List<GuestResponse>>(
                    entities),

            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<GuestResponse> GetByIdAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var guest =
            await db.Guests
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    guest =>
                        guest.HotelId == hotelId
                        &&
                        guest.Id == id
                        &&
                        !guest.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Guest not found.");

        return mapper.Map<GuestResponse>(
            guest);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<GuestResponse> CreateAsync(
        Guid hotelId,
        CreateGuestRequest request,
        CancellationToken cancellationToken)
    {
        var guest =
            new Guest(
                hotelId,
                request.FirstName,
                request.LastName,
                request.Phone,
                request.Email);

        db.Guests.Add(
            guest);

        await db.SaveChangesAsync(
            cancellationToken);

        return mapper.Map<GuestResponse>(
            guest);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<GuestResponse> UpdateAsync(
        Guid hotelId,
        Guid id,
        UpdateGuestRequest request,
        CancellationToken cancellationToken)
    {
        var guest =
            await db.Guests
                .FirstOrDefaultAsync(
                    guest =>
                        guest.HotelId == hotelId
                        &&
                        guest.Id == id
                        &&
                        !guest.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Guest not found.");

        guest.Update(
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Email);

        guest.MarkUpdated();

        await db.SaveChangesAsync(
            cancellationToken);

        return mapper.Map<GuestResponse>(
            guest);
    }
}
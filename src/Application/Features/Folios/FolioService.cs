using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Domain.Modules.Billing.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Folios;

public sealed record CreateFolioRequest(
    Guid ReservationId);

public sealed record AddFolioChargeRequest(
    string Category,
    string Description,
    decimal Amount);

public sealed class FolioService(
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
    // GET BY ID
    // =========================================================

    public async Task<Folio> GetAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        return await db.Folios
            .AsNoTracking()
            .Include(
                folio =>
                    folio.Charges)
            .FirstOrDefaultAsync(
                folio =>
                    folio.HotelId == hotelId
                    &&
                    folio.Id == id
                    &&
                    !folio.IsDeleted,
                cancellationToken)
            ?? throw new NotFoundException(
                "Folio not found.");
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<Folio> CreateAsync(
        CreateFolioRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        var reservation =
            await db.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    reservation =>
                        reservation.HotelId == hotelId
                        &&
                        reservation.Id == request.ReservationId
                        &&
                        !reservation.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Reservation not found.");

        var folioNumber =
            $"FOL-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

        var folio =
            new Folio(
                hotelId,
                reservation.BranchId,
                reservation.Id,
                reservation.GuestId,
                folioNumber);

        db.Folios.Add(
            folio);

        await db.SaveChangesAsync(
            cancellationToken);

        return folio;
    }

    // =========================================================
    // ADD CHARGE
    // =========================================================

    public async Task AddChargeAsync(
        Guid id,
        AddFolioChargeRequest request,
        CancellationToken cancellationToken)
    {
        var folio =
            await GetTrackedFolioAsync(
                id,
                cancellationToken);

        if (folio.IsClosed)
        {
            throw new ConflictException(
                "Folio is closed.");
        }

        var charge =
            new FolioCharge(
                folio.Id,
                request.Category.Trim(),
                request.Description.Trim(),
                request.Amount);

        db.FolioCharges.Add(
            charge);

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // CLOSE
    // =========================================================

    public async Task CloseAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var folio =
            await GetTrackedFolioAsync(
                id,
                cancellationToken);

        if (folio.IsClosed)
        {
            throw new ConflictException(
                "Folio is already closed.");
        }

        folio.Close();

        await db.SaveChangesAsync(
            cancellationToken);
    }

    // =========================================================
    // GET TRACKED FOLIO
    // =========================================================

    private async Task<Folio> GetTrackedFolioAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        return await db.Folios
            .FirstOrDefaultAsync(
                folio =>
                    folio.HotelId == hotelId
                    &&
                    folio.Id == id
                    &&
                    !folio.IsDeleted,
                cancellationToken)
            ?? throw new NotFoundException(
                "Folio not found.");
    }
}
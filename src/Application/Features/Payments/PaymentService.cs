using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Domain.Modules.Payments.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Payments;

public sealed class PaymentService(
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

    public async Task<PagedResult<Payment>> GetAllAsync(
        int pageNumber,
        int pageSize,
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

        var hotelId = HotelId;

        var query =
            db.Payments
                .AsNoTracking()
                .Where(
                    payment =>
                        payment.HotelId == hotelId
                        &&
                        !payment.IsDeleted);

        var totalItems =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderByDescending(
                    payment =>
                        payment.PaidAtUtc)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(
                    pageSize)
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<Payment>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<Payment> GetAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId = HotelId;

        return await db.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                payment =>
                    payment.HotelId == hotelId
                    &&
                    payment.Id == id
                    &&
                    !payment.IsDeleted,
                cancellationToken)
            ?? throw new NotFoundException(
                "Payment not found.");
    }
}
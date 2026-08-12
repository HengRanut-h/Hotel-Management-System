using AutoMapper;
using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Features.Invoices.Contracts;
using HotelManagement.Domain.Modules.Billing.Entities;
using HotelManagement.Domain.Modules.Payments.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Invoices.Services;

public sealed class InvoiceService(
    IApplicationDbContext db,
    IMapper mapper)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<PagedResult<InvoiceResponse>> GetAllAsync(
        Guid hotelId,
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

        var query =
            db.Invoices
                .AsNoTracking()
                .Include(
                    invoice =>
                        invoice.Guest)
                .Include(
                    invoice =>
                        invoice.Items)
                .Where(
                    invoice =>
                        invoice.HotelId == hotelId
                        &&
                        !invoice.IsDeleted);

        var totalItems =
            await query.CountAsync(
                cancellationToken);

        var entities =
            await query
                .OrderByDescending(
                    invoice =>
                        invoice.InvoiceDate)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(
                    pageSize)
                .ToListAsync(
                    cancellationToken);

        return new PagedResult<InvoiceResponse>
        {
            Items =
                mapper.Map<List<InvoiceResponse>>(
                    entities),

            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<InvoiceResponse> GetByIdAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var invoice =
            await db.Invoices
                .AsNoTracking()
                .Include(
                    invoice =>
                        invoice.Guest)
                .Include(
                    invoice =>
                        invoice.Items)
                .FirstOrDefaultAsync(
                    invoice =>
                        invoice.HotelId == hotelId
                        &&
                        invoice.Id == id
                        &&
                        !invoice.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Invoice not found.");

        return mapper.Map<InvoiceResponse>(
            invoice);
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<InvoiceResponse> CreateAsync(
        Guid hotelId,
        CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var guest =
            await db.Guests
                .FirstOrDefaultAsync(
                    guest =>
                        guest.Id == request.GuestId
                        &&
                        guest.HotelId == hotelId
                        &&
                        !guest.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Guest not found.");

        // =====================================================
        // RESERVATION VALIDATION
        // =====================================================

        if (request.ReservationId.HasValue)
        {
            var reservationExists =
                await db.Reservations.AnyAsync(
                    reservation =>
                        reservation.Id == request.ReservationId.Value
                        &&
                        reservation.GuestId == guest.Id
                        &&
                        reservation.HotelId == hotelId
                        &&
                        !reservation.IsDeleted,
                    cancellationToken);

            if (!reservationExists)
            {
                throw new NotFoundException(
                    "Reservation not found.");
            }
        }

        // =====================================================
        // NUMBER
        // =====================================================

        var invoiceNumber =
            $"INV-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

        // =====================================================
        // INVOICE
        // =====================================================

        var invoice =
            new Invoice(
                hotelId,
                request.GuestId,
                request.ReservationId,
                invoiceNumber,
                request.InvoiceDate,
                request.DueDate);

        foreach (var item in request.Items)
        {
            invoice.AddItem(
                item.Description,
                item.Quantity,
                item.UnitPrice,
                item.Unit);
        }

        invoice.SetAdjustments(
            request.DiscountAmount,
            request.TaxAmount);

        invoice.Issue();

        db.Invoices.Add(
            invoice);

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetByIdAsync(
            hotelId,
            invoice.Id,
            cancellationToken);
    }

    // =========================================================
    // ADD PAYMENT
    // =========================================================

    public async Task AddPaymentAsync(
        Guid hotelId,
        Guid invoiceId,
        AddPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var invoice =
            await db.Invoices
                .FirstOrDefaultAsync(
                    invoice =>
                        invoice.HotelId == hotelId
                        &&
                        invoice.Id == invoiceId
                        &&
                        !invoice.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Invoice not found.");

        if (request.Amount <= 0)
        {
            throw new ConflictException(
                "Payment amount must be greater than zero.");
        }

        if (request.Amount > invoice.BalanceAmount)
        {
            throw new ConflictException(
                "Payment amount cannot exceed invoice balance.");
        }

        var paymentNumber =
            $"PAY-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

        var payment =
            new Payment(
                hotelId,
                invoice.Id,
                paymentNumber,
                request.Amount,
                request.Method,
                request.ReferenceNumber);

        db.Payments.Add(
            payment);

        invoice.RegisterPayment(
            request.Amount);

        await db.SaveChangesAsync(
            cancellationToken);
    }
}
using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.AuditLogs;

public sealed class AuditLogService(
    IApplicationDbContext db,
    ICurrentUser currentUser)
{
    // =========================================================
    // GET AUDIT LOGS
    // =========================================================

    public async Task<object> GetAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.HotelId
            ?? throw new ForbiddenException(
                "Hotel context required.");

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
            db.AuditLogs
                .AsNoTracking()
                .Where(
                    auditLog =>
                        auditLog.HotelId == hotelId);

        // =====================================================
        // SEARCH
        // =====================================================

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm =
                search.Trim();

            query =
                query.Where(
                    auditLog =>
                        auditLog.Action.Contains(searchTerm)
                        ||
                        auditLog.EntityName.Contains(searchTerm));
        }

        // =====================================================
        // TOTAL ITEMS
        // =====================================================

        var totalItems =
            await query.CountAsync(
                cancellationToken);

        // =====================================================
        // PAGINATION
        // =====================================================

        var items =
            await query
                .OrderByDescending(
                    auditLog =>
                        auditLog.CreatedAtUtc)
                .Skip(
                    (pageNumber - 1) *
                    pageSize)
                .Take(
                    pageSize)
                .ToListAsync(
                    cancellationToken);

        var totalPages =
            (int)Math.Ceiling(
                totalItems /
                (double)pageSize);

        // =====================================================
        // RESPONSE
        // =====================================================

        return new
        {
            items,
            pageNumber,
            pageSize,
            totalItems,
            totalPages
        };
    }
}
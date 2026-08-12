using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Utilities.Contracts;
using HotelManagement.Application.Features.Utilities.Services;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.MeterReadings;

public sealed class MeterReadingsService(
    UtilityService utilities,
    IApplicationDbContext db,
    ICurrentUser currentUser)
{
    // =========================================================
    // RECORD READING
    // =========================================================

    public Task<MeterReadingResponse> RecordAsync(
        RecordMeterReadingRequest request,
        CancellationToken cancellationToken) =>
        utilities.RecordReadingAsync(
            currentUser.RequireHotelId(),
            request,
            cancellationToken);

    // =========================================================
    // GET BY METER
    // =========================================================

    public async Task<List<MeterReadingResponse>> GetByMeterAsync(
        Guid meterId,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.HotelId
            ?? throw new ForbiddenException(
                "Hotel context required.");

        var meterExists =
            await db.UtilityMeters.AnyAsync(
                utilityMeter =>
                    utilityMeter.Id == meterId
                    &&
                    utilityMeter.HotelId == hotelId
                    &&
                    !utilityMeter.IsDeleted,
                cancellationToken);

        if (!meterExists)
        {
            throw new NotFoundException(
                "Utility meter not found.");
        }

        return await db.MeterReadings
            .AsNoTracking()
            .Where(
                meterReading =>
                    meterReading.UtilityMeterId == meterId
                    &&
                    !meterReading.IsDeleted)
            .OrderByDescending(
                meterReading =>
                    meterReading.ReadingDate)
            .Select(
                meterReading =>
                    new MeterReadingResponse
                    {
                        Id = meterReading.Id,

                        UtilityMeterId =
                            meterReading.UtilityMeterId,

                        PreviousReading =
                            meterReading.PreviousReading,

                        CurrentReading =
                            meterReading.CurrentReading,

                        Usage =
                            meterReading.Usage,

                        RatePerUnit =
                            meterReading.RatePerUnit,

                        Amount =
                            meterReading.Amount,

                        ReadingDate =
                            meterReading.ReadingDate
                    })
            .ToListAsync(
                cancellationToken);
    }
}
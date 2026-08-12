using AutoMapper;
using HotelManagement.Application.Abstractions.Persistence;
using HotelManagement.Application.Common.Exceptions;
using HotelManagement.Application.Features.Utilities.Contracts;
using HotelManagement.Domain.Modules.Utilities.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Utilities.Services;

public sealed class UtilityService(
    IApplicationDbContext db,
    IMapper mapper)
{
    // =========================================================
    // GET METERS
    // =========================================================

    public async Task<List<UtilityMeterResponse>> GetMetersAsync(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        var entities =
            await db.UtilityMeters
                .AsNoTracking()
                .Include(
                    utilityMeter =>
                        utilityMeter.Room)
                .Where(
                    utilityMeter =>
                        utilityMeter.HotelId == hotelId
                        &&
                        !utilityMeter.IsDeleted)
                .OrderBy(
                    utilityMeter =>
                        utilityMeter.Room.RoomNumber)
                .ToListAsync(
                    cancellationToken);

        return mapper.Map<List<UtilityMeterResponse>>(
            entities);
    }

    // =========================================================
    // GET METER BY ID
    // =========================================================

    public async Task<UtilityMeterResponse> GetMeterByIdAsync(
        Guid hotelId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var utilityMeter =
            await db.UtilityMeters
                .AsNoTracking()
                .Include(
                    meter =>
                        meter.Room)
                .FirstOrDefaultAsync(
                    meter =>
                        meter.HotelId == hotelId
                        &&
                        meter.Id == id
                        &&
                        !meter.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Utility meter not found.");

        return mapper.Map<UtilityMeterResponse>(
            utilityMeter);
    }

    // =========================================================
    // CREATE METER
    // =========================================================

    public async Task<UtilityMeterResponse> CreateMeterAsync(
        Guid hotelId,
        CreateUtilityMeterRequest request,
        CancellationToken cancellationToken)
    {
        var room =
            await db.Rooms
                .FirstOrDefaultAsync(
                    room =>
                        room.Id == request.RoomId
                        &&
                        room.HotelId == hotelId
                        &&
                        !room.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Room not found.");

        var meterNumber =
            request.MeterNumber.Trim();

        var meterNumberExists =
            await db.UtilityMeters.AnyAsync(
                utilityMeter =>
                    utilityMeter.HotelId == hotelId
                    &&
                    utilityMeter.MeterNumber == meterNumber
                    &&
                    !utilityMeter.IsDeleted,
                cancellationToken);

        if (meterNumberExists)
        {
            throw new ConflictException(
                "Meter number already exists.");
        }

        if (request.RatePerUnit < 0)
        {
            throw new ConflictException(
                "Rate per unit cannot be negative.");
        }

        var utilityMeter =
            new UtilityMeter(
                hotelId,
                room.Id,
                request.Type,
                meterNumber,
                request.RatePerUnit);

        db.UtilityMeters.Add(
            utilityMeter);

        await db.SaveChangesAsync(
            cancellationToken);

        return await GetMeterByIdAsync(
            hotelId,
            utilityMeter.Id,
            cancellationToken);
    }

    // =========================================================
    // RECORD READING
    // =========================================================

    public async Task<MeterReadingResponse> RecordReadingAsync(
        Guid hotelId,
        RecordMeterReadingRequest request,
        CancellationToken cancellationToken)
    {
        var utilityMeter =
            await db.UtilityMeters
                .FirstOrDefaultAsync(
                    utilityMeter =>
                        utilityMeter.Id == request.UtilityMeterId
                        &&
                        utilityMeter.HotelId == hotelId
                        &&
                        !utilityMeter.IsDeleted,
                    cancellationToken)
            ?? throw new NotFoundException(
                "Utility meter not found.");

        if (request.CurrentReading < utilityMeter.LastReading)
        {
            throw new ConflictException(
                "Current reading cannot be lower than the previous reading.");
        }

        var meterReading =
            new MeterReading(
                utilityMeter.Id,
                utilityMeter.LastReading,
                request.CurrentReading,
                utilityMeter.RatePerUnit,
                request.ReadingDate);

        utilityMeter.SetLastReading(
            request.CurrentReading);

        db.MeterReadings.Add(
            meterReading);

        await db.SaveChangesAsync(
            cancellationToken);

        return mapper.Map<MeterReadingResponse>(
            meterReading);
    }
}
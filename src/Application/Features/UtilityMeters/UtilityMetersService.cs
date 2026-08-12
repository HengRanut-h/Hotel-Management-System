using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Utilities.Contracts;
using HotelManagement.Application.Features.Utilities.Services;

namespace HotelManagement.Application.Features.UtilityMeters;

public sealed class UtilityMetersService(
    UtilityService utilities,
    ICurrentUser currentUser)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public Task<List<UtilityMeterResponse>> GetAsync(
        CancellationToken cancellationToken) =>
        utilities.GetMetersAsync(
            currentUser.RequireHotelId(),
            cancellationToken);

    // =========================================================
    // GET BY ID
    // =========================================================

    public Task<UtilityMeterResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        utilities.GetMeterByIdAsync(
            currentUser.RequireHotelId(),
            id,
            cancellationToken);

    // =========================================================
    // CREATE
    // =========================================================

    public Task<UtilityMeterResponse> CreateAsync(
        CreateUtilityMeterRequest request,
        CancellationToken cancellationToken) =>
        utilities.CreateMeterAsync(
            currentUser.RequireHotelId(),
            request,
            cancellationToken);
}
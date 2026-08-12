using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Common.Services;
using HotelManagement.Domain.Modules.Services.Entities;

namespace HotelManagement.Application.Features.GuestRequests;

public sealed class GuestRequestsService(
    OperationalCrudService<GuestRequest> crud)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public Task<PagedResult<OperationalResponse>> GetAllAsync(
        OperationalQuery request,
        CancellationToken cancellationToken) =>
        crud.GetAllAsync(
            request,
            cancellationToken);

    // =========================================================
    // GET BY ID
    // =========================================================

    public Task<OperationalResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        crud.GetByIdAsync(
            id,
            cancellationToken);

    // =========================================================
    // CREATE
    // =========================================================

    public Task<OperationalResponse> CreateAsync(
        OperationalCreateRequest request,
        CancellationToken cancellationToken) =>
        crud.CreateAsync(
            request,
            (
                hotelId,
                branchId,
                referenceNumber,
                createRequest) =>
                new GuestRequest(
                    hotelId,
                    branchId,
                    referenceNumber,
                    createRequest.Title,
                    createRequest.Status,
                    createRequest.Notes,
                    createRequest.Amount,
                    createRequest.EventAtUtc
                        ?? DateTimeOffset.UtcNow,
                    createRequest.RelatedEntityId,
                    createRequest.RelatedEntityType),
            "REQ",
            cancellationToken);

    // =========================================================
    // UPDATE
    // =========================================================

    public Task<OperationalResponse> UpdateAsync(
        Guid id,
        OperationalUpdateRequest request,
        CancellationToken cancellationToken) =>
        crud.UpdateAsync(
            id,
            request,
            cancellationToken);

    // =========================================================
    // CHANGE STATUS
    // =========================================================

    public Task ChangeStatusAsync(
        Guid id,
        ChangeStatusRequest request,
        CancellationToken cancellationToken) =>
        crud.ChangeStatusAsync(
            id,
            request.Status,
            cancellationToken);

    // =========================================================
    // SOFT DELETE
    // =========================================================

    public Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        crud.DeleteAsync(
            id,
            cancellationToken);
}
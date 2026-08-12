using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Common.Services;
using HotelManagement.Domain.Modules.FrontDesk.Entities;

namespace HotelManagement.Application.Features.RoomAssignments;

public sealed class RoomAssignmentsService(
    OperationalCrudService<RoomAssignment> crud)
{
    public Task<PagedResult<OperationalResponse>> GetAllAsync(
        OperationalQuery request,
        CancellationToken cancellationToken) =>
        crud.GetAllAsync(
            request,
            cancellationToken);

    public Task<OperationalResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        crud.GetByIdAsync(
            id,
            cancellationToken);

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
                new RoomAssignment(
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
            "RA",
            cancellationToken);

    public Task<OperationalResponse> UpdateAsync(
        Guid id,
        OperationalUpdateRequest request,
        CancellationToken cancellationToken) =>
        crud.UpdateAsync(
            id,
            request,
            cancellationToken);

    public Task ChangeStatusAsync(
        Guid id,
        ChangeStatusRequest request,
        CancellationToken cancellationToken) =>
        crud.ChangeStatusAsync(
            id,
            request.Status,
            cancellationToken);

    public Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        crud.DeleteAsync(
            id,
            cancellationToken);
}
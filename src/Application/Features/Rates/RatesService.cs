using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Common.Services;
using HotelManagement.Domain.Modules.Rates.Entities;

namespace HotelManagement.Application.Features.Rates;

public sealed class RatesService(
    CatalogCrudService<RatePlan> crud)
{
    // =========================================================
    // GET ALL
    // =========================================================

    public Task<PagedResult<CatalogResponse>> GetAllAsync(
        CatalogQuery request,
        CancellationToken cancellationToken) =>
        crud.GetAllAsync(
            request,
            cancellationToken);

    // =========================================================
    // GET BY ID
    // =========================================================

    public Task<CatalogResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        crud.GetByIdAsync(
            id,
            cancellationToken);

    // =========================================================
    // CREATE
    // =========================================================

    public Task<CatalogResponse> CreateAsync(
        CatalogCreateRequest request,
        CancellationToken cancellationToken) =>
        crud.CreateAsync(
            request,
            (
                hotelId,
                branchId,
                createRequest) =>
                new RatePlan(
                    hotelId,
                    branchId,
                    createRequest.Name,
                    createRequest.Code,
                    createRequest.Description),
            cancellationToken);

    // =========================================================
    // UPDATE
    // =========================================================

    public Task<CatalogResponse> UpdateAsync(
        Guid id,
        CatalogUpdateRequest request,
        CancellationToken cancellationToken) =>
        crud.UpdateAsync(
            id,
            request,
            cancellationToken);

    // =========================================================
    // ACTIVE / INACTIVE
    // =========================================================

    public Task<CatalogResponse> SetActiveAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken) =>
        crud.SetActiveAsync(
            id,
            isActive,
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

    // =========================================================
    // RESTORE
    // =========================================================

    public Task<CatalogResponse> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        crud.RestoreAsync(
            id,
            cancellationToken);
}
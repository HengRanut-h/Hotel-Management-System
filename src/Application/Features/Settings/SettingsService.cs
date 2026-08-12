using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Common.Services;
using HotelManagement.Domain.Modules.Administration.Entities;

namespace HotelManagement.Application.Features.Settings;

public sealed class SettingsService(
    CatalogCrudService<SystemSetting> crud)
{
    public Task<PagedResult<CatalogResponse>> GetAllAsync(
        CatalogQuery request,
        CancellationToken cancellationToken) =>
        crud.GetAllAsync(
            request,
            cancellationToken);

    public Task<CatalogResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        crud.GetByIdAsync(
            id,
            cancellationToken);

    public Task<CatalogResponse> CreateAsync(
        CatalogCreateRequest request,
        CancellationToken cancellationToken) =>
        crud.CreateAsync(
            request,
            (
                hotelId,
                branchId,
                createRequest) =>
                new SystemSetting(
                    hotelId,
                    branchId,
                    createRequest.Name,
                    createRequest.Code,
                    createRequest.Description),
            cancellationToken);

    public Task<CatalogResponse> UpdateAsync(
        Guid id,
        CatalogUpdateRequest request,
        CancellationToken cancellationToken) =>
        crud.UpdateAsync(
            id,
            request,
            cancellationToken);

    public Task<CatalogResponse> SetActiveAsync(
        Guid id,
        bool isActive,
        CancellationToken cancellationToken) =>
        crud.SetActiveAsync(
            id,
            isActive,
            cancellationToken);

    public Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        crud.DeleteAsync(
            id,
            cancellationToken);

    public Task<CatalogResponse> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        crud.RestoreAsync(
            id,
            cancellationToken);
}
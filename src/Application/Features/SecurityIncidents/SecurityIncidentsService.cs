using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Queries;
using HotelManagement.Application.Common.Models.Requests;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Pagination;
using HotelManagement.Application.Common.Services;
using HotelManagement.Domain.Modules.Security.Entities;
namespace HotelManagement.Application.Features.SecurityIncidents;
public sealed class SecurityIncidentsService(OperationalCrudService<SecurityIncident> crud)
{
    public Task<PagedResult<OperationalResponse>> GetAllAsync(OperationalQuery q,CancellationToken ct)=>crud.GetAllAsync(q,ct);
    public Task<OperationalResponse> GetByIdAsync(Guid id,CancellationToken ct)=>crud.GetByIdAsync(id,ct);
    public Task<OperationalResponse> CreateAsync(OperationalCreateRequest r,CancellationToken ct)=>crud.CreateAsync(r,(hotelId,branchId,number,x)=>new SecurityIncident(hotelId,branchId,number,x.Title,x.Status,x.Notes,x.Amount,x.EventAtUtc??DateTimeOffset.UtcNow,x.RelatedEntityId,x.RelatedEntityType),"SEC",ct);
    public Task<OperationalResponse> UpdateAsync(Guid id,OperationalUpdateRequest r,CancellationToken ct)=>crud.UpdateAsync(id,r,ct);
    public Task ChangeStatusAsync(Guid id,ChangeStatusRequest r,CancellationToken ct)=>crud.ChangeStatusAsync(id,r.Status,ct);
    public Task DeleteAsync(Guid id,CancellationToken ct)=>crud.DeleteAsync(id,ct);
}

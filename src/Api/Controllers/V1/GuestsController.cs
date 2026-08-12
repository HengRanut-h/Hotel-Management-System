using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Guests.Contracts;
using HotelManagement.Application.Features.Guests.Services;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/guests")]
public sealed class GuestsController(
    GuestService service,
    ICurrentUser currentUser)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission(Permissions.Guests.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<GuestResponse>>>> GetAll(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.GetAllAsync(
                hotelId,
                pageNumber,
                pageSize,
                search,
                cancellationToken);

        var totalPages =
            response.PageSize == 0
                ? 0
                : (int)Math.Ceiling(
                    response.TotalItems /
                    (double)response.PageSize);

        return Ok(
            ApiResponse<IEnumerable<GuestResponse>>.Ok(
                response.Items,
                "Guests retrieved successfully.",
                meta: new
                {
                    pagination = new
                    {
                        response.PageNumber,
                        response.PageSize,
                        response.TotalItems,
                        totalPages,
                        hasPreviousPage =
                            response.PageNumber > 1,
                        hasNextPage =
                            response.PageNumber < totalPages
                    }
                },
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission(Permissions.Guests.View)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GuestResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.GetByIdAsync(
                hotelId,
                id,
                cancellationToken);

        return Ok(
            ApiResponse<GuestResponse>.Ok(
                response,
                "Guest retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission(Permissions.Guests.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<GuestResponse>>> Create(
        CreateGuestRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.CreateAsync(
                hotelId,
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = response.Id
            },
            ApiResponse<GuestResponse>.Created(
                response,
                "Guest created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission(Permissions.Guests.Update)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GuestResponse>>> Update(
        Guid id,
        UpdateGuestRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.UpdateAsync(
                hotelId,
                id,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<GuestResponse>.Updated(
                response,
                "Guest updated successfully.",
                HttpContext.TraceIdentifier));
    }
}
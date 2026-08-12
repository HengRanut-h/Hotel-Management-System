using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Reservations.Contracts;
using HotelManagement.Application.Features.Reservations.Services;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/reservations")]
public sealed class ReservationsController(
    ReservationService service,
    ICurrentUser currentUser)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission(Permissions.Reservations.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReservationResponse>>>> GetAll(
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
            ApiResponse<IEnumerable<ReservationResponse>>.Ok(
                response.Items,
                "Reservations retrieved successfully.",
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

    [HasPermission(Permissions.Reservations.View)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> GetById(
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
            ApiResponse<ReservationResponse>.Ok(
                response,
                "Reservation retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission(Permissions.Reservations.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReservationResponse>>> Create(
        CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var branchId =
            currentUser.RequireBranchId();

        var response =
            await service.CreateAsync(
                hotelId,
                branchId,
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = response.Id
            },
            ApiResponse<ReservationResponse>.Created(
                response,
                "Reservation created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CANCEL
    // =========================================================

    [HasPermission(Permissions.Reservations.Cancel)]
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<object?>>> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        await service.CancelAsync(
            hotelId,
            id,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Reservation cancelled successfully.",
                "RESERVATION_CANCELLED",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CHECK IN
    // =========================================================

    [HasPermission(Permissions.Reservations.CheckIn)]
    [HttpPost("{id:guid}/check-in")]
    public async Task<ActionResult<ApiResponse<object?>>> CheckIn(
        Guid id,
        CheckInRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        await service.CheckInAsync(
            hotelId,
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Reservation checked in successfully.",
                "RESERVATION_CHECKED_IN",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CHECK OUT
    // =========================================================

    [HasPermission(Permissions.Reservations.CheckOut)]
    [HttpPost("{id:guid}/check-out")]
    public async Task<ActionResult<ApiResponse<object?>>> CheckOut(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        await service.CheckOutAsync(
            hotelId,
            id,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Reservation checked out successfully.",
                "RESERVATION_CHECKED_OUT",
                HttpContext.TraceIdentifier));
    }
}
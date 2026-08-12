using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Rooms.Contracts;
using HotelManagement.Application.Features.Rooms.Services;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/rooms")]
public sealed class RoomsController(
    RoomService service,
    ICurrentUser currentUser)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission(
        Permissions.Rooms.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RoomResponse>>>> GetAll(
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
            ApiResponse<IEnumerable<RoomResponse>>.Ok(
                response.Items,
                "Rooms retrieved successfully.",
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

    [HasPermission(
        Permissions.Rooms.View)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> GetById(
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
            ApiResponse<RoomResponse>.Ok(
                response,
                "Room retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission(
        Permissions.Rooms.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> Create(
        CreateRoomRequest request,
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
            ApiResponse<RoomResponse>.Created(
                response,
                "Room created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HasPermission(
        Permissions.Rooms.Update)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> Update(
        Guid id,
        UpdateRoomRequest request,
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
            ApiResponse<RoomResponse>.Updated(
                response,
                "Room updated successfully.",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CHANGE STATUS
    // =========================================================

    [HasPermission(
        Permissions.Rooms.ChangeStatus)]
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<object?>>> ChangeStatus(
        Guid id,
        ChangeRoomStatusRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        await service.ChangeStatusAsync(
            hotelId,
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Room status updated successfully.",
                "ROOM_STATUS_CHANGED",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HasPermission(
        Permissions.Rooms.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        await service.DeleteAsync(
            hotelId,
            id,
            cancellationToken);

        return NoContent();
    }
}
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Notifications;
using HotelManagement.Domain.Modules.Notifications.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/notifications")]
public sealed class NotificationsController(
    NotificationsService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Notification>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAsync(
                cancellationToken);

        return Ok(
            ApiResponse<IEnumerable<Notification>>.Ok(
                response,
                "Notifications retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Notification>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetByIdAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<Notification>.Ok(
                response,
                "Notification retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // MARK AS READ
    // =========================================================

    [HttpPatch("{id:guid}/read")]
    public async Task<ActionResult<ApiResponse<object?>>> Read(
        Guid id,
        CancellationToken cancellationToken)
    {
        await service.ReadAsync(
            id,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Notification marked as read successfully.",
                "NOTIFICATION_READ",
                HttpContext.TraceIdentifier));
    }
}
using HotelManagement.Application.Features.Availability;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/availability")]
[Produces("application/json")]
public sealed class AvailabilityController : ControllerBase
{
    private readonly AvailabilityService _availabilityService;

    public AvailabilityController(
        AvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    // =========================================================
    // SEARCH AVAILABLE ROOMS
    // GET /api/v1/availability
    //
    // Example:
    // /api/v1/availability
    //      ?checkIn=2026-08-20
    //      &checkOut=2026-08-22
    //
    // Optional:
    //      &roomTypeId=<guid>
    // =========================================================

    [HasPermission("availability.view")]
    [HttpGet]
    [ProducesResponseType(
        typeof(List<AvailabilityRoom>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<AvailabilityRoom>>> Search(
        [FromQuery] DateOnly checkIn,
        [FromQuery] DateOnly checkOut,
        [FromQuery] Guid? roomTypeId = null,
        CancellationToken cancellationToken = default)
    {
        // =====================================================
        // DATE VALIDATION
        // =====================================================

        if (checkIn == default)
        {
            return BadRequest(new
            {
                message = "Check-in date is required."
            });
        }

        if (checkOut == default)
        {
            return BadRequest(new
            {
                message = "Check-out date is required."
            });
        }

        if (checkOut <= checkIn)
        {
            return BadRequest(new
            {
                message =
                    "Check-out date must be after check-in date."
            });
        }

        // =====================================================
        // SEARCH
        // =====================================================

        var rooms =
            await _availabilityService.SearchAsync(
                checkIn,
                checkOut,
                roomTypeId,
                cancellationToken);

        return Ok(rooms);
    }
}
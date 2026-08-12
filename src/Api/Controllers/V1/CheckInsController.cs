using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.CheckIns;
using HotelManagement.Domain.Modules.FrontDesk.Entities;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/check-ins")]
public sealed class CheckInsController(
    CheckInService service)
    : ControllerBase
{
    // =========================================================
    // CHECK IN
    // =========================================================

    [HasPermission("reservations.check-in")]
    [HttpPost("{reservationId:guid}")]
    public async Task<ActionResult<ApiResponse<CheckInRecord>>> CheckIn(
        Guid reservationId,
        CheckInCommand request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.ExecuteAsync(
                reservationId,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<CheckInRecord>.Action(
                response,
                "Check-in completed successfully.",
                "CHECK_IN_COMPLETED",
                HttpContext.TraceIdentifier));
    }
}
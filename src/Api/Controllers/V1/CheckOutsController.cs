using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.CheckOuts;
using HotelManagement.Domain.Modules.FrontDesk.Entities;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/check-outs")]
public sealed class CheckOutsController(
    CheckOutService service)
    : ControllerBase
{
    // =========================================================
    // CHECK OUT
    // =========================================================

    [HasPermission("reservations.check-out")]
    [HttpPost("{reservationId:guid}")]
    public async Task<ActionResult<ApiResponse<CheckOutRecord>>> CheckOut(
        Guid reservationId,
        CancellationToken cancellationToken)
    {
        var response =
            await service.ExecuteAsync(
                reservationId,
                cancellationToken);

        return Ok(
            ApiResponse<CheckOutRecord>.Action(
                response,
                "Check-out completed successfully.",
                "CHECK_OUT_COMPLETED",
                HttpContext.TraceIdentifier));
    }
}
using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.MasterData.Services;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/master-data")]
public sealed class MasterDataController(
    MasterDataService service,
    ICurrentUser currentUser)
    : ControllerBase
{
    // =========================================================
    // GET MASTER DATA
    // =========================================================

    [HasPermission(Permissions.Dashboard.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> Get(
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.GetAsync(
                hotelId,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                response,
                "Master data retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }
}
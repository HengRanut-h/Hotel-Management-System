using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Payments;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Domain.Modules.Payments.Entities;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/payments")]
public sealed class PaymentsController(
    PaymentService service)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission(Permissions.Payments.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Payment>>>> GetAll(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var response =
            await service.GetAllAsync(
                pageNumber,
                pageSize,
                cancellationToken);

        var totalPages =
            response.PageSize == 0
                ? 0
                : (int)Math.Ceiling(
                    response.TotalItems /
                    (double)response.PageSize);

        return Ok(
            ApiResponse<IEnumerable<Payment>>.Ok(
                response.Items,
                "Payments retrieved successfully.",
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

    [HasPermission(Permissions.Payments.View)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Payment>>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<Payment>.Ok(
                response,
                "Payment retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }
}
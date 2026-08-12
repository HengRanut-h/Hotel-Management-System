using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Features.Folios;
using HotelManagement.Domain.Modules.Billing.Entities;
using HotelManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/folios")]
public sealed class FoliosController(
    FolioService service)
    : ControllerBase
{
    // =========================================================
    // GET BY ID
    // =========================================================

    [HasPermission("folios.view")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Folio>>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await service.GetAsync(
                id,
                cancellationToken);

        return Ok(
            ApiResponse<Folio>.Ok(
                response,
                "Folio retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission("folios.create")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Folio>>> Create(
        CreateFolioRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await service.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(Get),
            new
            {
                id = response.Id
            },
            ApiResponse<Folio>.Created(
                response,
                "Folio created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // ADD CHARGE
    // =========================================================

    [HasPermission("folios.update")]
    [HttpPost("{id:guid}/charges")]
    public async Task<ActionResult<ApiResponse<object?>>> AddCharge(
        Guid id,
        AddFolioChargeRequest request,
        CancellationToken cancellationToken)
    {
        await service.AddChargeAsync(
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Folio charge added successfully.",
                "FOLIO_CHARGE_ADDED",
                HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CLOSE
    // =========================================================

    [HasPermission("folios.update")]
    [HttpPost("{id:guid}/close")]
    public async Task<ActionResult<ApiResponse<object?>>> Close(
        Guid id,
        CancellationToken cancellationToken)
    {
        await service.CloseAsync(
            id,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Folio closed successfully.",
                "FOLIO_CLOSED",
                HttpContext.TraceIdentifier));
    }
}
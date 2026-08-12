using HotelManagement.Application.Abstractions.Security;
using HotelManagement.Application.Common.Models;
using HotelManagement.Application.Common.Models.Responses;
using HotelManagement.Application.Common.Security;
using HotelManagement.Application.Features.Invoices.Contracts;
using HotelManagement.Application.Features.Invoices.Services;
using HotelManagement.Domain.Modules.Identity.Constants;
using HotelManagement.Infrastructure.Authorization;
using HotelManagement.Infrastructure.Pdf;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Api.Controllers.V1;

[ApiController]
[Route("api/v1/invoices")]
public sealed class InvoicesController(
    InvoiceService service,
    ICurrentUser currentUser,
    InvoicePdfGenerator pdfGenerator)
    : ControllerBase
{
    // =========================================================
    // GET ALL
    // =========================================================

    [HasPermission(Permissions.Invoices.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<InvoiceResponse>>>> GetAll(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var response =
            await service.GetAllAsync(
                hotelId,
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
            ApiResponse<IEnumerable<InvoiceResponse>>.Ok(
                response.Items,
                "Invoices retrieved successfully.",
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

    [HasPermission(Permissions.Invoices.View)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> GetById(
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
            ApiResponse<InvoiceResponse>.Ok(
                response,
                "Invoice retrieved successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HasPermission(Permissions.Invoices.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<InvoiceResponse>>> Create(
        CreateInvoiceRequest request,
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
            ApiResponse<InvoiceResponse>.Created(
                response,
                "Invoice created successfully.",
                traceId: HttpContext.TraceIdentifier));
    }

    // =========================================================
    // DOWNLOAD PDF
    // =========================================================

    [HasPermission(Permissions.Invoices.View)]
    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> Pdf(
        Guid id,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        var invoice =
            await service.GetByIdAsync(
                hotelId,
                id,
                cancellationToken);

        var bytes =
            pdfGenerator.Generate(
                invoice.InvoiceNumber,
                invoice.GuestName,
                invoice.TotalAmount,
                invoice.PaidAmount);

        return File(
            bytes,
            "application/pdf",
            $"{invoice.InvoiceNumber}.pdf");
    }

    // =========================================================
    // ADD PAYMENT
    // =========================================================

    [HasPermission(Permissions.Payments.Create)]
    [HttpPost("{id:guid}/payments")]
    public async Task<ActionResult<ApiResponse<object?>>> AddPayment(
        Guid id,
        AddPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var hotelId =
            currentUser.RequireHotelId();

        await service.AddPaymentAsync(
            hotelId,
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<object?>.Action(
                null,
                "Payment added to invoice successfully.",
                "PAYMENT_ADDED",
                HttpContext.TraceIdentifier));
    }
}
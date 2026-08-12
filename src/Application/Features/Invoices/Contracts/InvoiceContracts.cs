using System.ComponentModel.DataAnnotations;
using HotelManagement.Domain.Modules.Payments.Enums;

namespace HotelManagement.Application.Features.Invoices.Contracts;

public sealed class CreateInvoiceRequest
{
    public Guid GuestId { get; set; }
    public Guid? ReservationId { get; set; }
    public DateOnly InvoiceDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? DueDate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    [MinLength(1)] public List<CreateInvoiceItemRequest> Items { get; set; } = [];
}

public sealed class CreateInvoiceItemRequest
{
    [Required, MaxLength(250)] public string Description { get; set; } = string.Empty;
    [Range(0.0001, 999999)] public decimal Quantity { get; set; } = 1;
    [Range(0, 999999)] public decimal UnitPrice { get; set; }
    [MaxLength(20)] public string? Unit { get; set; }
}

public sealed class AddPaymentRequest
{
    [Range(0.01, 999999999)] public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    [MaxLength(200)] public string? ReferenceNumber { get; set; }
}

public sealed class InvoiceResponse
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public Guid? ReservationId { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<InvoiceItemResponse> Items { get; set; } = [];
}

public sealed class InvoiceItemResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

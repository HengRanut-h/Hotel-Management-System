using HotelManagement.Domain.Common.Entities;
using HotelManagement.Domain.Modules.Billing.Entities;
using HotelManagement.Domain.Modules.Hotels.Entities;
using HotelManagement.Domain.Modules.Payments.Enums;

namespace HotelManagement.Domain.Modules.Payments.Entities;

public sealed class Payment : AuditableEntity
{
    private Payment() { }

    public Payment(Guid hotelId, Guid invoiceId, string paymentNumber, decimal amount, PaymentMethod method, string? referenceNumber)
    {
        HotelId = hotelId;
        InvoiceId = invoiceId;
        PaymentNumber = paymentNumber;
        Amount = amount;
        Method = method;
        ReferenceNumber = referenceNumber;
        PaidAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid HotelId { get; private set; }
    public Hotel Hotel { get; private set; } = null!;
    public Guid InvoiceId { get; private set; }
    public Invoice Invoice { get; private set; } = null!;
    public string PaymentNumber { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public DateTimeOffset PaidAtUtc { get; private set; }
}

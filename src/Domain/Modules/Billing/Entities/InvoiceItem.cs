using HotelManagement.Domain.Common.Entities;

namespace HotelManagement.Domain.Modules.Billing.Entities;

public sealed class InvoiceItem : BaseEntity
{
    private InvoiceItem() { }

    internal InvoiceItem(Guid invoiceId, string description, decimal quantity, decimal unitPrice, string? unit)
    {
        InvoiceId = invoiceId;
        Description = description.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        Unit = unit;
    }

    public Guid InvoiceId { get; private set; }
    public Invoice Invoice { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string? Unit { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total => Quantity * UnitPrice;
}

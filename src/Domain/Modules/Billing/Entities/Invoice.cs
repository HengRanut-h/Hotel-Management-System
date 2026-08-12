using HotelManagement.Domain.Common.Entities;
using HotelManagement.Domain.Modules.Billing.Enums;
using HotelManagement.Domain.Modules.Guests.Entities;
using HotelManagement.Domain.Modules.Hotels.Entities;
using HotelManagement.Domain.Modules.Reservations.Entities;

namespace HotelManagement.Domain.Modules.Billing.Entities;

public sealed class Invoice : AuditableEntity
{
    private Invoice() { }

    public Invoice(Guid hotelId, Guid guestId, Guid? reservationId, string invoiceNumber, DateOnly invoiceDate, DateOnly? dueDate)
    {
        HotelId = hotelId;
        GuestId = guestId;
        ReservationId = reservationId;
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        Status = InvoiceStatus.Draft;
    }

    public Guid HotelId { get; private set; }
    public Hotel Hotel { get; private set; } = null!;
    public Guid GuestId { get; private set; }
    public Guest Guest { get; private set; } = null!;
    public Guid? ReservationId { get; private set; }
    public Reservation? Reservation { get; private set; }
    public string InvoiceNumber { get; private set; } = string.Empty;
    public DateOnly InvoiceDate { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal BalanceAmount => TotalAmount - PaidAmount;
    public InvoiceStatus Status { get; private set; }
    public ICollection<InvoiceItem> Items { get; private set; } = new List<InvoiceItem>();

    public void AddItem(string description, decimal quantity, decimal unitPrice, string? unit = null)
    {
        Items.Add(new InvoiceItem(Id, description, quantity, unitPrice, unit));
        Recalculate();
    }

    public void SetAdjustments(decimal discountAmount, decimal taxAmount)
    {
        DiscountAmount = Math.Max(0, discountAmount);
        TaxAmount = Math.Max(0, taxAmount);
        Recalculate();
    }

    public void Issue()
    {
        Recalculate();
        Status = InvoiceStatus.Issued;
    }

    public void RegisterPayment(decimal amount)
    {
        PaidAmount += amount;
        Status = BalanceAmount <= 0 ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
    }

    public void MarkOverdue()
    {
        if (Status is InvoiceStatus.Issued or InvoiceStatus.PartiallyPaid)
            Status = InvoiceStatus.Overdue;
    }

    private void Recalculate()
    {
        Subtotal = Items.Sum(x => x.Total);
        TotalAmount = Math.Max(0, Subtotal - DiscountAmount + TaxAmount);
    }
}

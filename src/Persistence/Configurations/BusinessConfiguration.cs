using HotelManagement.Domain.Modules.Billing.Entities;
using HotelManagement.Domain.Modules.Guests.Entities;
using HotelManagement.Domain.Modules.Housekeeping.Entities;
using HotelManagement.Domain.Modules.Maintenance.Entities;
using HotelManagement.Domain.Modules.Payments.Entities;
using HotelManagement.Domain.Modules.Reservations.Entities;
using HotelManagement.Domain.Modules.Utilities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Persistence.Configurations;

public sealed class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> b)
    {
        b.ToTable("guests");
        b.HasKey(x => x.Id);

        b.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.Email)
            .HasMaxLength(200);

        b.Property(x => x.Phone)
            .HasMaxLength(50);

        b.Ignore(x => x.FullName);

        b.HasOne(x => x.Hotel)
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class ReservationConfiguration
    : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> b)
    {
        b.ToTable("reservations");
        b.HasKey(x => x.Id);

        b.Property(x => x.ReservationNumber)
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.NightlyRate)
            .HasPrecision(18, 2);

        b.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        b.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        b.Ignore(x => x.Nights);

        b.HasIndex(x => x.ReservationNumber)
            .IsUnique();

        b.HasOne(x => x.Guest)
            .WithMany()
            .HasForeignKey(x => x.GuestId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.RoomType)
            .WithMany()
            .HasForeignKey(x => x.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Room)
            .WithMany()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Hotel)
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class InvoiceConfiguration
    : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> b)
    {
        b.ToTable("invoices");
        b.HasKey(x => x.Id);

        b.Property(x => x.InvoiceNumber)
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.Subtotal).HasPrecision(18, 2);
        b.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        b.Property(x => x.TaxAmount).HasPrecision(18, 2);
        b.Property(x => x.TotalAmount).HasPrecision(18, 2);
        b.Property(x => x.PaidAmount).HasPrecision(18, 2);

        b.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        b.Ignore(x => x.BalanceAmount);

        b.HasIndex(x => x.InvoiceNumber)
            .IsUnique();

        b.HasOne(x => x.Hotel)
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Guest)
            .WithMany()
            .HasForeignKey(x => x.GuestId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Reservation)
            .WithMany()
            .HasForeignKey(x => x.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class InvoiceItemConfiguration
    : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> b)
    {
        b.ToTable("invoice_items");
        b.HasKey(x => x.Id);

        b.Property(x => x.Description)
            .HasMaxLength(250)
            .IsRequired();

        b.Property(x => x.Quantity)
            .HasPrecision(18, 4);

        b.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        b.Ignore(x => x.Total);

        b.HasOne(x => x.Invoice)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class PaymentConfiguration
    : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("payments");
        b.HasKey(x => x.Id);

        b.Property(x => x.PaymentNumber)
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.Amount)
            .HasPrecision(18, 2);

        b.Property(x => x.Method)
            .HasConversion<string>()
            .HasMaxLength(30);

        b.HasIndex(x => x.PaymentNumber)
            .IsUnique();

        b.HasOne(x => x.Hotel)
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Invoice)
            .WithMany()
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class UtilityMeterConfiguration
    : IEntityTypeConfiguration<UtilityMeter>
{
    public void Configure(EntityTypeBuilder<UtilityMeter> b)
    {
        b.ToTable("utility_meters");
        b.HasKey(x => x.Id);

        b.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        b.Property(x => x.MeterNumber)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.RatePerUnit)
            .HasPrecision(18, 4);

        b.Property(x => x.LastReading)
            .HasPrecision(18, 4);

        b.HasIndex(x => x.MeterNumber)
            .IsUnique();

        b.HasOne(x => x.Hotel)
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Room)
            .WithMany()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class MeterReadingConfiguration
    : IEntityTypeConfiguration<MeterReading>
{
    public void Configure(EntityTypeBuilder<MeterReading> b)
    {
        b.ToTable("meter_readings");
        b.HasKey(x => x.Id);

        b.Property(x => x.PreviousReading).HasPrecision(18, 4);
        b.Property(x => x.CurrentReading).HasPrecision(18, 4);
        b.Property(x => x.Usage).HasPrecision(18, 4);
        b.Property(x => x.RatePerUnit).HasPrecision(18, 4);
        b.Property(x => x.Amount).HasPrecision(18, 2);

        b.HasOne(x => x.UtilityMeter)
            .WithMany()
            .HasForeignKey(x => x.UtilityMeterId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class HousekeepingTaskConfiguration
    : IEntityTypeConfiguration<HousekeepingTask>
{
    public void Configure(EntityTypeBuilder<HousekeepingTask> b)
    {
        b.ToTable("housekeeping_tasks");
        b.HasKey(x => x.Id);

        b.Property(x => x.TaskType)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.Priority)
            .HasMaxLength(30);

        b.Property(x => x.Status)
            .HasMaxLength(30);

        b.HasOne(x => x.Room)
            .WithMany()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class MaintenanceRequestConfiguration
    : IEntityTypeConfiguration<MaintenanceRequest>
{
    public void Configure(EntityTypeBuilder<MaintenanceRequest> b)
    {
        b.ToTable("maintenance_requests");
        b.HasKey(x => x.Id);

        b.Property(x => x.Category)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired();

        b.Property(x => x.Priority)
            .HasMaxLength(30);

        b.Property(x => x.Status)
            .HasMaxLength(30);

        b.HasOne(x => x.Room)
            .WithMany()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

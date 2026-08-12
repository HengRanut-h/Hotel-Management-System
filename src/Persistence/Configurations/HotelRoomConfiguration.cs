using HotelManagement.Domain.Modules.Hotels.Entities;
using HotelManagement.Domain.Modules.Rooms.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Persistence.Configurations;

public sealed class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> b)
    {
        b.ToTable("hotels");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Code)
            .HasMaxLength(30)
            .IsRequired();

        b.Property(x => x.Currency)
            .HasMaxLength(10)
            .IsRequired();

        b.HasIndex(x => x.Code).IsUnique();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class HotelBranchConfiguration
    : IEntityTypeConfiguration<HotelBranch>
{
    public void Configure(EntityTypeBuilder<HotelBranch> b)
    {
        b.ToTable("hotel_branches");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Code)
            .HasMaxLength(30)
            .IsRequired();

        b.HasIndex(x => new
        {
            x.HotelId,
            x.Code
        }).IsUnique();

        b.HasOne(x => x.Hotel)
            .WithMany(x => x.Branches)
            .HasForeignKey(x => x.HotelId);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class RoomTypeConfiguration
    : IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> b)
    {
        b.ToTable("room_types");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.Code)
            .HasMaxLength(30)
            .IsRequired();

        b.Property(x => x.BaseRate)
            .HasPrecision(18, 2);

        b.HasIndex(x => new
        {
            x.HotelId,
            x.Code
        }).IsUnique();

        b.HasOne(x => x.Hotel)
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> b)
    {
        b.ToTable("rooms");
        b.HasKey(x => x.Id);

        b.Property(x => x.RoomNumber)
            .HasMaxLength(20)
            .IsRequired();

        b.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        b.HasIndex(x => new
        {
            x.BranchId,
            x.RoomNumber
        }).IsUnique();

        b.HasOne(x => x.Hotel)
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.RoomType)
            .WithMany()
            .HasForeignKey(x => x.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

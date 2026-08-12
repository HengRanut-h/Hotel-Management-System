using HotelManagement.Domain.Modules.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");
        b.HasKey(x => x.Id);

        b.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.PasswordHash)
            .HasMaxLength(1000)
            .IsRequired();

        b.HasIndex(x => x.Email).IsUnique();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("roles");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.NormalizedName)
            .HasMaxLength(100)
            .IsRequired();

        b.HasIndex(x => x.NormalizedName).IsUnique();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.ToTable("permissions");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        b.HasIndex(x => x.Name).IsUnique();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> b)
    {
        b.ToTable("user_roles");

        b.HasKey(x => new
        {
            x.UserId,
            x.RoleId
        });

        b.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId);

        b.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId);
    }
}

public sealed class RolePermissionConfiguration
    : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> b)
    {
        b.ToTable("role_permissions");

        b.HasKey(x => new
        {
            x.RoleId,
            x.PermissionId
        });

        b.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId);

        b.HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId);
    }
}

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("refresh_tokens");
        b.HasKey(x => x.Id);

        b.Property(x => x.TokenHash)
            .HasMaxLength(128)
            .IsRequired();

        b.HasIndex(x => x.TokenHash).IsUnique();

        b.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId);
    }
}

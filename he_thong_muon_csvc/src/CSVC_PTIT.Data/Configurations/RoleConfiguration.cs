namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Fluent API cho bảng roles
/// Dữ liệu cố định: SV, DT, QL, AD
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleId)
            .HasColumnName("role_id");

        builder.Property(r => r.RoleCode)
            .HasColumnName("role_code")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(r => r.RoleName)
            .HasColumnName("role_name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(200);

        // Unique: mã vai trò không trùng
        builder.HasIndex(r => r.RoleCode)
            .IsUnique();
    }
}

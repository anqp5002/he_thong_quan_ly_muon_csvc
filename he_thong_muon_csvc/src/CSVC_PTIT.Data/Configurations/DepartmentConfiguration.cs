namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Fluent API cho bảng departments
/// </summary>
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.DepartmentId);

        builder.Property(d => d.DepartmentId)
            .HasColumnName("department_id");

        builder.Property(d => d.DepartmentCode)
            .HasColumnName("department_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.DepartmentName)
            .HasColumnName("department_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.Description)
            .HasColumnName("description")
            .HasMaxLength(200);

        // Unique: mã đơn vị không trùng
        builder.HasIndex(d => d.DepartmentCode)
            .IsUnique();
    }
}

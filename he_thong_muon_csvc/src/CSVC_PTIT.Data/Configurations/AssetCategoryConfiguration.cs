namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Fluent API cho bảng asset_categories
/// </summary>
public class AssetCategoryConfiguration : IEntityTypeConfiguration<AssetCategory>
{
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.ToTable("asset_categories");

        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.CategoryId)
            .HasColumnName("category_id");

        builder.Property(c => c.CategoryCode)
            .HasColumnName("category_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.CategoryName)
            .HasColumnName("category_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasMaxLength(200);

        // Unique: mã loại CSVC không trùng
        builder.HasIndex(c => c.CategoryCode)
            .IsUnique();
    }
}

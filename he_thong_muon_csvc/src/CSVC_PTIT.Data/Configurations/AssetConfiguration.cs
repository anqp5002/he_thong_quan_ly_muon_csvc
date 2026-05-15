namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Fluent API cho bảng assets
/// FK → asset_categories, rooms. Enum mapping cho management_mode, condition_status, availability_status
/// </summary>
public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("assets");

        builder.HasKey(a => a.AssetId);

        builder.Property(a => a.AssetId)
            .HasColumnName("asset_id");

        builder.Property(a => a.AssetCode)
            .HasColumnName("asset_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.AssetName)
            .HasColumnName("asset_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.ManagementMode)
            .HasColumnName("management_mode")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.Unit)
            .HasColumnName("unit")
            .HasMaxLength(20);

        builder.Property(a => a.TotalQuantity)
            .HasColumnName("total_quantity");

        builder.Property(a => a.AvailableQuantity)
            .HasColumnName("available_quantity");

        builder.Property(a => a.SerialNumber)
            .HasColumnName("serial_number")
            .HasMaxLength(50);

        builder.Property(a => a.Brand)
            .HasColumnName("brand")
            .HasMaxLength(50);

        builder.Property(a => a.Model)
            .HasColumnName("model")
            .HasMaxLength(50);

        builder.Property(a => a.ConditionStatus)
            .HasColumnName("condition_status")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.AvailabilityStatus)
            .HasColumnName("availability_status")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        // FK: AssetCategory
        builder.Property(a => a.CategoryId)
            .HasColumnName("category_id");

        builder.HasOne(a => a.Category)
            .WithMany(c => c.Assets)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK: Room (nullable — CSVC có thể không thuộc phòng nào)
        builder.Property(a => a.CurrentRoomId)
            .HasColumnName("current_room_id");

        builder.HasOne(a => a.CurrentRoom)
            .WithMany(r => r.Assets)
            .HasForeignKey(a => a.CurrentRoomId)
            .OnDelete(DeleteBehavior.SetNull);

        // ===== Indexes =====
        builder.HasIndex(a => a.AssetCode).IsUnique();
    }
}

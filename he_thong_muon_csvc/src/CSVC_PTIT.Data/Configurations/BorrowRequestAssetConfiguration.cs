namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Fluent API cho bảng borrow_request_assets (bảng trung gian)
/// FK → borrow_requests + assets
/// </summary>
public class BorrowRequestAssetConfiguration : IEntityTypeConfiguration<BorrowRequestAsset>
{
    public void Configure(EntityTypeBuilder<BorrowRequestAsset> builder)
    {
        builder.ToTable("borrow_request_assets");

        builder.HasKey(bra => bra.RequestAssetId);

        builder.Property(bra => bra.RequestAssetId)
            .HasColumnName("request_asset_id");

        builder.Property(bra => bra.QuantityRequested)
            .HasColumnName("quantity_requested");

        builder.Property(bra => bra.QuantityApproved)
            .HasColumnName("quantity_approved");

        builder.Property(bra => bra.QuantityCheckedOut)
            .HasColumnName("quantity_checked_out");

        builder.Property(bra => bra.QuantityReturned)
            .HasColumnName("quantity_returned");

        builder.Property(bra => bra.ItemNote)
            .HasColumnName("item_note")
            .HasMaxLength(200);

        // FK: BorrowRequest
        builder.Property(bra => bra.RequestId)
            .HasColumnName("request_id");

        builder.HasOne(bra => bra.BorrowRequest)
            .WithMany(br => br.BorrowRequestAssets)
            .HasForeignKey(bra => bra.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK: Asset
        builder.Property(bra => bra.AssetId)
            .HasColumnName("asset_id");

        builder.HasOne(bra => bra.Asset)
            .WithMany(a => a.BorrowRequestAssets)
            .HasForeignKey(bra => bra.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== Indexes =====
        builder.HasIndex(bra => new { bra.RequestId, bra.AssetId }).IsUnique();
    }
}

namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CheckoutItemConfiguration : IEntityTypeConfiguration<CheckoutItem>
{
    public void Configure(EntityTypeBuilder<CheckoutItem> builder)
    {
        builder.ToTable("checkout_items");
        builder.HasKey(ci => ci.CheckoutItemId);

        builder.Property(ci => ci.CheckoutItemId).HasColumnName("checkout_item_id");
        builder.Property(ci => ci.ConditionBefore).HasColumnName("condition_before").HasMaxLength(200);
        builder.Property(ci => ci.Quantity).HasColumnName("quantity");
        builder.Property(ci => ci.IsReturned).HasColumnName("is_returned").HasDefaultValue(false);
        builder.Property(ci => ci.CheckoutId).HasColumnName("checkout_id");
        builder.Property(ci => ci.RequestAssetId).HasColumnName("request_asset_id");
        builder.Property(ci => ci.AssetId).HasColumnName("asset_id");

        builder.HasOne(ci => ci.Checkout).WithMany(c => c.CheckoutItems)
            .HasForeignKey(ci => ci.CheckoutId).OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.BorrowRequestAsset).WithMany(bra => bra.CheckoutItems)
            .HasForeignKey(ci => ci.RequestAssetId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ci => ci.Asset).WithMany(a => a.CheckoutItems)
            .HasForeignKey(ci => ci.AssetId).OnDelete(DeleteBehavior.Restrict);
    }
}

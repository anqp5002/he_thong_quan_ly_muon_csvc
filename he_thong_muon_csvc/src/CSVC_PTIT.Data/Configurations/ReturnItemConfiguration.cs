namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReturnItemConfiguration : IEntityTypeConfiguration<ReturnItem>
{
    public void Configure(EntityTypeBuilder<ReturnItem> builder)
    {
        builder.ToTable("return_items");
        builder.HasKey(ri => ri.ReturnItemId);

        builder.Property(ri => ri.ReturnItemId).HasColumnName("return_item_id");
        builder.Property(ri => ri.ConditionAfter).HasColumnName("condition_after").HasMaxLength(200);
        builder.Property(ri => ri.QuantityReturned).HasColumnName("quantity_returned");
        builder.Property(ri => ri.IsDamaged).HasColumnName("is_damaged").HasDefaultValue(false);
        builder.Property(ri => ri.IsLost).HasColumnName("is_lost").HasDefaultValue(false);
        builder.Property(ri => ri.DamageNote).HasColumnName("damage_note").HasMaxLength(500);
        builder.Property(ri => ri.ReturnId).HasColumnName("return_id");
        builder.Property(ri => ri.CheckoutItemId).HasColumnName("checkout_item_id");
        builder.Property(ri => ri.AssetId).HasColumnName("asset_id");

        builder.HasOne(ri => ri.Return).WithMany(r => r.ReturnItems)
            .HasForeignKey(ri => ri.ReturnId).OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.CheckoutItem).WithMany(ci => ci.ReturnItems)
            .HasForeignKey(ri => ri.CheckoutItemId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ri => ri.Asset).WithMany(a => a.ReturnItems)
            .HasForeignKey(ri => ri.AssetId).OnDelete(DeleteBehavior.Restrict);
    }
}

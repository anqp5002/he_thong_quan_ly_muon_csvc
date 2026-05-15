namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CheckoutConfiguration : IEntityTypeConfiguration<Checkout>
{
    public void Configure(EntityTypeBuilder<Checkout> builder)
    {
        builder.ToTable("checkouts");
        builder.HasKey(c => c.CheckoutId);

        builder.Property(c => c.CheckoutId).HasColumnName("checkout_id");
        builder.Property(c => c.CheckoutCode).HasColumnName("checkout_code").HasMaxLength(20).IsRequired();
        builder.Property(c => c.CheckoutAt).HasColumnName("checkout_at");
        builder.Property(c => c.CheckoutNote).HasColumnName("checkout_note").HasMaxLength(500);
        builder.Property(c => c.RequestId).HasColumnName("request_id");
        builder.Property(c => c.CheckedOutBy).HasColumnName("checked_out_by");
        builder.Property(c => c.CheckedOutTo).HasColumnName("checked_out_to");

        builder.HasOne(c => c.BorrowRequest).WithMany(br => br.Checkouts)
            .HasForeignKey(c => c.RequestId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CheckedOutByUser).WithMany()
            .HasForeignKey(c => c.CheckedOutBy).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CheckedOutToUser).WithMany()
            .HasForeignKey(c => c.CheckedOutTo).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CheckoutCode).IsUnique();
    }
}

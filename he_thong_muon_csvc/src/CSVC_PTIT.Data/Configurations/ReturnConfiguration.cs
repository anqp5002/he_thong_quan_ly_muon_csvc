namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReturnConfiguration : IEntityTypeConfiguration<Return>
{
    public void Configure(EntityTypeBuilder<Return> builder)
    {
        builder.ToTable("returns");
        builder.HasKey(r => r.ReturnId);

        builder.Property(r => r.ReturnId).HasColumnName("return_id");
        builder.Property(r => r.ReturnedAt).HasColumnName("returned_at");
        builder.Property(r => r.ReturnNote).HasColumnName("return_note").HasMaxLength(500);
        builder.Property(r => r.CheckoutId).HasColumnName("checkout_id");
        builder.Property(r => r.ReceivedBy).HasColumnName("received_by");
        builder.Property(r => r.ReturnedBy).HasColumnName("returned_by");

        builder.HasOne(r => r.Checkout).WithMany(c => c.Returns)
            .HasForeignKey(r => r.CheckoutId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReceivedByUser).WithMany()
            .HasForeignKey(r => r.ReceivedBy).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReturnedByUser).WithMany()
            .HasForeignKey(r => r.ReturnedBy).OnDelete(DeleteBehavior.Restrict);
    }
}

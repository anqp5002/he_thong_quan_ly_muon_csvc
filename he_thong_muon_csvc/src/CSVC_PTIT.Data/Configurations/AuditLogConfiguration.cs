using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSVC_PTIT.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(e => e.LogId);

        builder.Property(e => e.Action).IsRequired().HasMaxLength(50);
        builder.Property(e => e.TargetType).HasMaxLength(100);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

        // Relates to User
        builder.HasOne(e => e.User)
            .WithMany() // User không nhất thiết phải chứa collection AuditLogs nếu không query ngược
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

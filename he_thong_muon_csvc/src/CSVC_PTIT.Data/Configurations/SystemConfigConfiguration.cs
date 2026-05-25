using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSVC_PTIT.Data.Configurations;

public class SystemConfigConfiguration : IEntityTypeConfiguration<SystemConfig>
{
    public void Configure(EntityTypeBuilder<SystemConfig> builder)
    {
        builder.ToTable("SystemConfigs");

        builder.HasKey(e => e.ConfigId);

        builder.Property(e => e.ConfigKey).IsRequired().HasMaxLength(100);
        builder.HasIndex(e => e.ConfigKey).IsUnique(); // Đảm bảo key cấu hình là duy nhất

        builder.Property(e => e.ConfigValue).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(255);
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
    }
}

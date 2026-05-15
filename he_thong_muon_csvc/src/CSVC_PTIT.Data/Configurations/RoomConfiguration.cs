namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Fluent API cho bảng rooms
/// Enum mapping cho room_type, status
/// </summary>
public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("rooms");

        builder.HasKey(r => r.RoomId);

        builder.Property(r => r.RoomId)
            .HasColumnName("room_id");

        builder.Property(r => r.RoomCode)
            .HasColumnName("room_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.RoomName)
            .HasColumnName("room_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Building)
            .HasColumnName("building")
            .HasMaxLength(50);

        builder.Property(r => r.FloorNo)
            .HasColumnName("floor_no");

        builder.Property(r => r.Capacity)
            .HasColumnName("capacity");

        builder.Property(r => r.RoomType)
            .HasColumnName("room_type")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20);

        // ===== Indexes =====
        builder.HasIndex(r => r.RoomCode).IsUnique();
    }
}

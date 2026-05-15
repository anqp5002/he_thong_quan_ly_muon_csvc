namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Fluent API cho bảng borrow_request_rooms (bảng trung gian)
/// FK → borrow_requests + rooms
/// </summary>
public class BorrowRequestRoomConfiguration : IEntityTypeConfiguration<BorrowRequestRoom>
{
    public void Configure(EntityTypeBuilder<BorrowRequestRoom> builder)
    {
        builder.ToTable("borrow_request_rooms");

        builder.HasKey(brr => brr.RequestRoomId);

        builder.Property(brr => brr.RequestRoomId)
            .HasColumnName("request_room_id");

        builder.Property(brr => brr.UsageNote)
            .HasColumnName("usage_note")
            .HasMaxLength(200);

        // FK: BorrowRequest
        builder.Property(brr => brr.RequestId)
            .HasColumnName("request_id");

        builder.HasOne(brr => brr.BorrowRequest)
            .WithMany(br => br.BorrowRequestRooms)
            .HasForeignKey(brr => brr.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK: Room
        builder.Property(brr => brr.RoomId)
            .HasColumnName("room_id");

        builder.HasOne(brr => brr.Room)
            .WithMany(r => r.BorrowRequestRooms)
            .HasForeignKey(brr => brr.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== Indexes =====
        builder.HasIndex(brr => new { brr.RequestId, brr.RoomId }).IsUnique();
    }
}

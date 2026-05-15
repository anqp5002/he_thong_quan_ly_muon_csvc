namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Fluent API cho bảng borrow_requests — BẢNG TRUNG TÂM của hệ thống
/// 2 FK đến users (requester + approver), nhiều enum mapping
/// </summary>
public class BorrowRequestConfiguration : IEntityTypeConfiguration<BorrowRequest>
{
    public void Configure(EntityTypeBuilder<BorrowRequest> builder)
    {
        builder.ToTable("borrow_requests");

        builder.HasKey(br => br.RequestId);

        builder.Property(br => br.RequestId)
            .HasColumnName("request_id");

        builder.Property(br => br.RequestCode)
            .HasColumnName("request_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(br => br.RequestType)
            .HasColumnName("request_type")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(br => br.ContactPhone)
            .HasColumnName("contact_phone")
            .HasMaxLength(20);

        builder.Property(br => br.Title)
            .HasColumnName("title")
            .HasMaxLength(200);

        builder.Property(br => br.Purpose)
            .HasColumnName("purpose")
            .HasMaxLength(500);

        builder.Property(br => br.BorrowStartAt)
            .HasColumnName("borrow_start_at");

        builder.Property(br => br.BorrowEndAt)
            .HasColumnName("borrow_end_at");

        builder.Property(br => br.ExpectedReturnAt)
            .HasColumnName("expected_return_at");

        builder.Property(br => br.ActualReturnAt)
            .HasColumnName("actual_return_at");

        builder.Property(br => br.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(br => br.PriorityLevel)
            .HasColumnName("priority_level")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(br => br.RequestNote)
            .HasColumnName("request_note")
            .HasMaxLength(500);

        builder.Property(br => br.RejectReason)
            .HasColumnName("reject_reason")
            .HasMaxLength(500);

        builder.Property(br => br.ApprovedAt)
            .HasColumnName("approved_at");

        builder.Property(br => br.CreatedAt)
            .HasColumnName("created_at");

        // FK: Requester (người tạo đơn — SV hoặc BT LCĐ)
        builder.Property(br => br.RequesterId)
            .HasColumnName("requester_id");

        builder.HasOne(br => br.Requester)
            .WithMany(u => u.BorrowRequests)
            .HasForeignKey(br => br.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK: Approver (người duyệt — QL_CSVC, nullable)
        builder.Property(br => br.ApprovedBy)
            .HasColumnName("approved_by");

        builder.HasOne(br => br.Approver)
            .WithMany(u => u.ApprovedRequests)
            .HasForeignKey(br => br.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== Indexes =====
        builder.HasIndex(br => br.RequestCode).IsUnique();
        builder.HasIndex(br => br.Status);
        builder.HasIndex(br => br.RequesterId);
    }
}

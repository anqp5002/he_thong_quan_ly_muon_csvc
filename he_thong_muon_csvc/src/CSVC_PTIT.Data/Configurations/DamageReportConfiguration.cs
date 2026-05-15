namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DamageReportConfiguration : IEntityTypeConfiguration<DamageReport>
{
    public void Configure(EntityTypeBuilder<DamageReport> builder)
    {
        builder.ToTable("damage_reports");
        builder.HasKey(dr => dr.ReportId);

        builder.Property(dr => dr.ReportId).HasColumnName("report_id");
        builder.Property(dr => dr.Severity).HasColumnName("severity")
            .HasConversion<string>().HasMaxLength(20);
        builder.Property(dr => dr.IncidentType).HasColumnName("incident_type")
            .HasConversion<string>().HasMaxLength(20);
        builder.Property(dr => dr.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(dr => dr.EstimatedCompensation).HasColumnName("estimated_compensation")
            .HasColumnType("decimal(18,2)");
        builder.Property(dr => dr.ActualCompensation).HasColumnName("actual_compensation")
            .HasColumnType("decimal(18,2)");
        builder.Property(dr => dr.Status).HasColumnName("status")
            .HasConversion<string>().HasMaxLength(20);
        builder.Property(dr => dr.ReportedAt).HasColumnName("reported_at");
        builder.Property(dr => dr.ResolvedAt).HasColumnName("resolved_at");
        builder.Property(dr => dr.ResolutionNote).HasColumnName("resolution_note").HasMaxLength(500);
        builder.Property(dr => dr.RequestId).HasColumnName("request_id");
        builder.Property(dr => dr.ReturnItemId).HasColumnName("return_item_id");
        builder.Property(dr => dr.AssetId).HasColumnName("asset_id");
        builder.Property(dr => dr.ReportedBy).HasColumnName("reported_by");
        builder.Property(dr => dr.ResponsibleUserId).HasColumnName("responsible_user_id");

        // FK: BorrowRequest
        builder.HasOne(dr => dr.BorrowRequest).WithMany(br => br.DamageReports)
            .HasForeignKey(dr => dr.RequestId).OnDelete(DeleteBehavior.Restrict);

        // FK: ReturnItem (nullable)
        builder.HasOne(dr => dr.ReturnItem).WithMany(ri => ri.DamageReports)
            .HasForeignKey(dr => dr.ReturnItemId).OnDelete(DeleteBehavior.SetNull);

        // FK: Asset
        builder.HasOne(dr => dr.Asset).WithMany(a => a.DamageReports)
            .HasForeignKey(dr => dr.AssetId).OnDelete(DeleteBehavior.Restrict);

        // FK: ReportedBy (QL_CSVC)
        builder.HasOne(dr => dr.ReportedByUser).WithMany()
            .HasForeignKey(dr => dr.ReportedBy).OnDelete(DeleteBehavior.Restrict);

        // FK: ResponsibleUser (SV/CN CLB)
        builder.HasOne(dr => dr.ResponsibleUser).WithMany()
            .HasForeignKey(dr => dr.ResponsibleUserId).OnDelete(DeleteBehavior.Restrict);

        // ===== Indexes =====
        builder.HasIndex(dr => dr.Status);
    }
}

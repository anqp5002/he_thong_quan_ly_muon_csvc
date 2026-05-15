namespace CSVC_PTIT.Data.Entities;

using CSVC_PTIT.Data.Enums;

/// <summary>
/// Bảng 14: Biên bản sự cố hỏng hóc / mất CSVC (QL_BM05)
/// Được tạo khi QL_CSVC phát hiện CSVC bị hỏng hoặc mất trong quá trình đối soát
/// </summary>
public class DamageReport
{
    public int ReportId { get; set; }

    /// <summary>Mức độ: Low/Medium/High</summary>
    public Severity Severity { get; set; }

    /// <summary>Loại sự cố: Damage (hỏng) / Loss (mất)</summary>
    public IncidentType IncidentType { get; set; }

    /// <summary>Mô tả chi tiết sự cố</summary>
    public string? Description { get; set; }

    /// <summary>Chi phí bồi thường dự kiến (VNĐ)</summary>
    public decimal EstimatedCompensation { get; set; }

    /// <summary>Chi phí bồi thường thực tế (VNĐ)</summary>
    public decimal ActualCompensation { get; set; }

    /// <summary>Trạng thái xử lý: Open/Confirmed/Closed</summary>
    public DamageReportStatus Status { get; set; } = DamageReportStatus.Open;

    public DateTime ReportedAt { get; set; } = DateTime.Now;
    public DateTime? ResolvedAt { get; set; }

    /// <summary>Ghi chú khi đóng biên bản</summary>
    public string? ResolutionNote { get; set; }

    // ===== FK =====

    /// <summary>FK → Đơn mượn liên quan</summary>
    public int RequestId { get; set; }
    public BorrowRequest BorrowRequest { get; set; } = null!;

    /// <summary>FK → Dòng trả CSVC phát hiện sự cố</summary>
    public int? ReturnItemId { get; set; }
    public ReturnItem? ReturnItem { get; set; }

    /// <summary>FK → CSVC bị hỏng/mất</summary>
    public int AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    /// <summary>FK → Người lập biên bản (QL_CSVC)</summary>
    public int ReportedBy { get; set; }
    public User ReportedByUser { get; set; } = null!;

    /// <summary>FK → Người chịu trách nhiệm (SV/CN CLB)</summary>
    public int ResponsibleUserId { get; set; }
    public User ResponsibleUser { get; set; } = null!;
}

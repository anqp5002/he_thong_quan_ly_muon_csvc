namespace CSVC_PTIT.Data.Entities;

/// <summary>
/// Bảng 8: Chi tiết CSVC trong đơn mượn (bảng trung gian)
/// 1 Đơn mượn có thể yêu cầu nhiều loại CSVC
/// VD: Đơn DT-001 mượn 2 Micro + 5 Ghế nhựa
/// </summary>
public class BorrowRequestAsset
{
    public int RequestAssetId { get; set; }

    /// <summary>Số lượng yêu cầu ban đầu</summary>
    public int QuantityRequested { get; set; }

    /// <summary>Số lượng được duyệt (có thể ít hơn yêu cầu)</summary>
    public int QuantityApproved { get; set; }

    /// <summary>Số lượng thực tế đã bàn giao</summary>
    public int QuantityCheckedOut { get; set; }

    /// <summary>Số lượng đã trả</summary>
    public int QuantityReturned { get; set; }

    public string? ItemNote { get; set; }

    // ===== FK =====

    /// <summary>FK → Đơn mượn</summary>
    public int RequestId { get; set; }
    public BorrowRequest BorrowRequest { get; set; } = null!;

    /// <summary>FK → CSVC cần mượn</summary>
    public int AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    // ===== Navigation ngược =====
    public ICollection<CheckoutItem> CheckoutItems { get; set; } = new List<CheckoutItem>();
}

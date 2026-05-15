namespace CSVC_PTIT.Data.Entities;

/// <summary>
/// Bảng 11: Chi tiết CSVC trong phiếu bàn giao
/// Mỗi item ghi nhận tình trạng CSVC tại thời điểm giao
/// </summary>
public class CheckoutItem
{
    public int CheckoutItemId { get; set; }

    /// <summary>Tình trạng CSVC khi giao: "Tốt", "Có vết xước nhẹ"...</summary>
    public string? ConditionBefore { get; set; }

    /// <summary>Số lượng bàn giao</summary>
    public int Quantity { get; set; }

    /// <summary>Đã trả hay chưa (dùng để track từng item)</summary>
    public bool IsReturned { get; set; } = false;

    // ===== FK =====

    /// <summary>FK → Phiếu bàn giao</summary>
    public int CheckoutId { get; set; }
    public Checkout Checkout { get; set; } = null!;

    /// <summary>FK → Dòng chi tiết trong đơn mượn</summary>
    public int RequestAssetId { get; set; }
    public BorrowRequestAsset BorrowRequestAsset { get; set; } = null!;

    /// <summary>FK → CSVC cụ thể</summary>
    public int AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    // ===== Navigation ngược =====
    public ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();
}

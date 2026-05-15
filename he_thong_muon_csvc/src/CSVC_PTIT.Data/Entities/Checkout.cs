namespace CSVC_PTIT.Data.Entities;

/// <summary>
/// Bảng 10: Phiếu bàn giao CSVC (QL_BM03)
/// Khi đơn được duyệt, QL_CSVC tạo phiếu bàn giao để giao CSVC cho người mượn
/// </summary>
public class Checkout
{
    public int CheckoutId { get; set; }

    /// <summary>Mã phiếu bàn giao: BG-0087</summary>
    public string CheckoutCode { get; set; } = string.Empty;

    /// <summary>Thời điểm bàn giao</summary>
    public DateTime CheckoutAt { get; set; }

    public string? CheckoutNote { get; set; }

    // ===== FK =====

    /// <summary>FK → Đơn mượn gốc</summary>
    public int RequestId { get; set; }
    public BorrowRequest BorrowRequest { get; set; } = null!;

    /// <summary>FK → Người bàn giao (QL_CSVC)</summary>
    public int CheckedOutBy { get; set; }
    public User CheckedOutByUser { get; set; } = null!;

    /// <summary>FK → Người nhận CSVC (SV/BT LCĐ)</summary>
    public int CheckedOutTo { get; set; }
    public User CheckedOutToUser { get; set; } = null!;

    // ===== Navigation ngược =====

    /// <summary>Danh sách CSVC trong phiếu bàn giao</summary>
    public ICollection<CheckoutItem> CheckoutItems { get; set; } = new List<CheckoutItem>();

    /// <summary>Các phiếu trả của phiếu bàn giao này</summary>
    public ICollection<Return> Returns { get; set; } = new List<Return>();
}

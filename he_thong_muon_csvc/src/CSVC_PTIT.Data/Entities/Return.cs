namespace CSVC_PTIT.Data.Entities;

/// <summary>
/// Bảng 12: Phiếu nhận trả CSVC (QL_BM04)
/// Khi người mượn trả CSVC, QL_CSVC tạo phiếu trả để đối soát
/// </summary>
public class Return
{
    public int ReturnId { get; set; }

    /// <summary>Thời điểm trả thực tế</summary>
    public DateTime ReturnedAt { get; set; }

    public string? ReturnNote { get; set; }

    // ===== FK =====

    /// <summary>FK → Phiếu bàn giao gốc (để đối soát)</summary>
    public int CheckoutId { get; set; }
    public Checkout Checkout { get; set; } = null!;

    /// <summary>FK → Người nhận trả (QL_CSVC)</summary>
    public int ReceivedBy { get; set; }
    public User ReceivedByUser { get; set; } = null!;

    /// <summary>FK → Người trả (SV/BT LCĐ)</summary>
    public int ReturnedBy { get; set; }
    public User ReturnedByUser { get; set; } = null!;

    // ===== Navigation ngược =====

    /// <summary>Chi tiết từng CSVC được trả</summary>
    public ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();
}

namespace CSVC_PTIT.Data.Entities;

/// <summary>
/// Bảng 13: Chi tiết CSVC trong phiếu trả
/// So sánh tình trạng trước (khi giao) vs sau (khi trả) để phát hiện hỏng/mất
/// </summary>
public class ReturnItem
{
    public int ReturnItemId { get; set; }

    /// <summary>Tình trạng CSVC sau khi trả: "Tốt", "Bị nứt"...</summary>
    public string? ConditionAfter { get; set; }

    /// <summary>Số lượng thực tế trả</summary>
    public int QuantityReturned { get; set; }

    /// <summary>CSVC có bị hỏng không?</summary>
    public bool IsDamaged { get; set; } = false;

    /// <summary>CSVC có bị mất không?</summary>
    public bool IsLost { get; set; } = false;

    /// <summary>Ghi chú hỏng/mất: "Nứt chân ghế", "Mất micro MC-015"</summary>
    public string? DamageNote { get; set; }

    // ===== FK =====

    /// <summary>FK → Phiếu trả</summary>
    public int ReturnId { get; set; }
    public Return Return { get; set; } = null!;

    /// <summary>FK → Dòng CSVC trong phiếu bàn giao (để đối soát)</summary>
    public int CheckoutItemId { get; set; }
    public CheckoutItem CheckoutItem { get; set; } = null!;

    /// <summary>FK → CSVC cụ thể</summary>
    public int AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    // ===== Navigation ngược =====
    public ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();
}

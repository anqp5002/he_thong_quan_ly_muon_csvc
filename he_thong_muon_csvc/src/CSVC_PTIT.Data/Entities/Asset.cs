namespace CSVC_PTIT.Data.Entities;

using CSVC_PTIT.Data.Enums;

/// <summary>
/// Bảng 5: Cơ sở vật chất (CSVC)
/// VD: MC-015 (Micro không dây), GN-042 (Ghế nhựa #42), PH-B401 (Phòng B401)
/// </summary>
public class Asset
{
    public int AssetId { get; set; }

    /// <summary>Mã CSVC duy nhất: MC-015, GN-042, PH-B401</summary>
    public string AssetCode { get; set; } = string.Empty;

    public string AssetName { get; set; } = string.Empty;

    /// <summary>
    /// Chế độ quản lý:
    /// - Quantity: quản lý theo số lượng (ghế nhựa: 50 cái)
    /// - Item: quản lý theo từng cái (phòng B401: 1 phòng)
    /// </summary>
    public ManagementMode ManagementMode { get; set; }

    /// <summary>Đơn vị tính: cái, bộ, phòng...</summary>
    public string? Unit { get; set; }

    /// <summary>Tổng số lượng ban đầu</summary>
    public int TotalQuantity { get; set; }

    /// <summary>Số lượng hiện đang khả dụng (tự động tăng/giảm khi mượn/trả)</summary>
    public int AvailableQuantity { get; set; }

    public string? SerialNumber { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }

    /// <summary>Tình trạng vật lý: Good/Fair/Damaged</summary>
    public ConditionStatus ConditionStatus { get; set; } = ConditionStatus.Good;

    /// <summary>Có sẵn để mượn không: Available/Unavailable</summary>
    public AvailabilityStatus AvailabilityStatus { get; set; } = AvailabilityStatus.Available;

    public string? Description { get; set; }

    // ===== FK =====

    /// <summary>FK → Loại CSVC</summary>
    public int CategoryId { get; set; }
    public AssetCategory Category { get; set; } = null!;

    /// <summary>FK → Phòng hiện tại đang đặt CSVC này</summary>
    public int? CurrentRoomId { get; set; }
    public Room? CurrentRoom { get; set; }

    // ===== Navigation ngược =====
    public ICollection<BorrowRequestAsset> BorrowRequestAssets { get; set; } = new List<BorrowRequestAsset>();
    public ICollection<CheckoutItem> CheckoutItems { get; set; } = new List<CheckoutItem>();
    public ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();
    public ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();
}

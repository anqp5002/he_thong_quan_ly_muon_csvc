namespace CSVC_PTIT.Data.Entities;

/// <summary>
/// Bảng 4: Danh mục loại CSVC
/// VD: Thiết bị điện tử, Bàn ghế, Phòng học, Dụng cụ...
/// </summary>
public class AssetCategory
{
    public int CategoryId { get; set; }

    /// <summary>Mã loại: TBDT, BG, PH, DC...</summary>
    public string CategoryCode { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation: 1 Category có nhiều Assets
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}

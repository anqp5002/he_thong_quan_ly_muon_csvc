namespace CSVC_PTIT.Data.Entities;

using CSVC_PTIT.Data.Enums;

/// <summary>
/// Bảng 6: Phòng học / Hội trường / Phòng thí nghiệm
/// VD: B401 (Phòng học tầng 4 tòa B), HTD (Hội trường D)
/// </summary>
public class Room
{
    public int RoomId { get; set; }

    /// <summary>Mã phòng: B401, HTD, A301</summary>
    public string RoomCode { get; set; } = string.Empty;

    public string RoomName { get; set; } = string.Empty;

    /// <summary>Tòa nhà: Tòa A, Tòa B, Tòa D</summary>
    public string? Building { get; set; }

    /// <summary>Tầng</summary>
    public int? FloorNo { get; set; }

    /// <summary>Sức chứa (số người)</summary>
    public int? Capacity { get; set; }

    /// <summary>Loại phòng: Classroom/Hall/Lab</summary>
    public RoomType RoomType { get; set; }

    /// <summary>Trạng thái: Available/InUse/Maintenance</summary>
    public RoomStatus Status { get; set; } = RoomStatus.Available;

    /// <summary>Tình trạng phòng (ví dụ: Mới, Cũ, Đang sửa chữa)</summary>
    public string? Condition { get; set; } = "Mới";

    // ===== Navigation ngược =====

    /// <summary>Các CSVC đang đặt trong phòng này</summary>
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();

    /// <summary>Các yêu cầu mượn phòng</summary>
    public ICollection<BorrowRequestRoom> BorrowRequestRooms { get; set; } = new List<BorrowRequestRoom>();
}

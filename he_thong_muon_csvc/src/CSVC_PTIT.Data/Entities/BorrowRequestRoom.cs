namespace CSVC_PTIT.Data.Entities;

/// <summary>
/// Bảng 9: Chi tiết phòng trong đơn mượn (bảng trung gian)
/// VD: Đơn DT-001 mượn Hội trường D + Phòng A301
/// </summary>
public class BorrowRequestRoom
{
    public int RequestRoomId { get; set; }

    public string? UsageNote { get; set; }

    // ===== FK =====

    /// <summary>FK → Đơn mượn</summary>
    public int RequestId { get; set; }
    public BorrowRequest BorrowRequest { get; set; } = null!;

    /// <summary>FK → Phòng cần mượn</summary>
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
}

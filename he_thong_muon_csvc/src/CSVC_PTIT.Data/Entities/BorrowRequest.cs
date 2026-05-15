namespace CSVC_PTIT.Data.Entities;

using CSVC_PTIT.Data.Enums;

/// <summary>
/// Bảng 7: Đơn yêu cầu mượn CSVC
/// Đây là bảng TRUNG TÂM của toàn bộ hệ thống.
/// Mỗi đơn đi qua vòng đời: Pending → Approved → CheckedOut → Returned
/// </summary>
public class BorrowRequest
{
    public int RequestId { get; set; }

    /// <summary>Mã đơn tự sinh: SV-001, DT-001</summary>
    public string RequestCode { get; set; } = string.Empty;

    /// <summary>Loại đơn: InClass (trong giờ) / OffHours (ngoài giờ)</summary>
    public RequestType RequestType { get; set; }

    /// <summary>SĐT liên hệ của người mượn</summary>
    public string? ContactPhone { get; set; }

    /// <summary>Tiêu đề/Tên sự kiện: "Workshop An ninh mạng"</summary>
    public string? Title { get; set; }

    /// <summary>Mục đích sử dụng</summary>
    public string? Purpose { get; set; }

    /// <summary>Thời gian bắt đầu mượn</summary>
    public DateTime BorrowStartAt { get; set; }

    /// <summary>Thời gian kết thúc mượn</summary>
    public DateTime BorrowEndAt { get; set; }

    /// <summary>Giờ phải trả (dự kiến)</summary>
    public DateTime ExpectedReturnAt { get; set; }

    /// <summary>Giờ trả thực tế (ghi nhận khi trả)</summary>
    public DateTime? ActualReturnAt { get; set; }

    /// <summary>Trạng thái vòng đời đơn</summary>
    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    public PriorityLevel PriorityLevel { get; set; } = PriorityLevel.Normal;

    /// <summary>Ghi chú khi tạo đơn</summary>
    public string? RequestNote { get; set; }

    /// <summary>Lý do từ chối (nếu bị reject)</summary>
    public string? RejectReason { get; set; }

    /// <summary>Thời điểm duyệt đơn</summary>
    public DateTime? ApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // ===== FK =====

    /// <summary>FK → Người tạo đơn (SV hoặc BT LCĐ)</summary>
    public int RequesterId { get; set; }
    public User Requester { get; set; } = null!;

    /// <summary>FK → Người duyệt đơn (QL_CSVC — quản lý phòng CSVC)</summary>
    public int? ApprovedBy { get; set; }
    public User? Approver { get; set; }

    // ===== Navigation ngược =====

    /// <summary>Danh sách CSVC trong đơn (bảng trung gian)</summary>
    public ICollection<BorrowRequestAsset> BorrowRequestAssets { get; set; } = new List<BorrowRequestAsset>();

    /// <summary>Danh sách phòng trong đơn (bảng trung gian)</summary>
    public ICollection<BorrowRequestRoom> BorrowRequestRooms { get; set; } = new List<BorrowRequestRoom>();

    /// <summary>Các phiếu bàn giao của đơn này</summary>
    public ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();

    /// <summary>Các biên bản sự cố liên quan</summary>
    public ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();
}

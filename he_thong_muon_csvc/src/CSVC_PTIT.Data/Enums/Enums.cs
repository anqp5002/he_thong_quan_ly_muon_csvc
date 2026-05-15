namespace CSVC_PTIT.Data.Enums;

/// <summary>
/// Loại người mượn CSVC
/// </summary>
public enum BorrowerType
{
    Student,     // Sinh viên
    Lecturer,    // Giảng viên
    Club,        // Câu lạc bộ
    Unit,        // Đơn vị (LCĐ, Đoàn)
    External,    // Bên ngoài
    Staff,       // Nhân viên
    Admin        // Quản trị viên
}

/// <summary>
/// Trạng thái tài khoản người dùng
/// </summary>
public enum UserStatus
{
    Active,      // Đang hoạt động
    Inactive,    // Ngưng hoạt động
    Locked       // Bị khóa
}

/// <summary>
/// Chế độ quản lý CSVC: theo số lượng hoặc theo từng cái
/// VD: Ghế nhựa → quản lý theo số lượng (50 cái)
/// VD: Phòng B401 → quản lý theo từng cái (1 phòng)
/// </summary>
public enum ManagementMode
{
    Quantity,    // Quản lý theo số lượng (ghế, micro, bàn...)
    Item         // Quản lý theo đơn vị (phòng, hội trường...)
}

/// <summary>
/// Tình trạng vật lý của CSVC
/// </summary>
public enum ConditionStatus
{
    Good,        // Tốt
    Fair,        // Khá (có dấu hiệu cũ)
    Damaged      // Hỏng
}

/// <summary>
/// CSVC có sẵn để mượn hay không
/// </summary>
public enum AvailabilityStatus
{
    Available,   // Khả dụng — có thể mượn
    Unavailable  // Không khả dụng — đang được mượn/bảo trì
}

/// <summary>
/// Loại phòng
/// </summary>
public enum RoomType
{
    Classroom,   // Phòng học
    Hall,        // Hội trường
    Lab          // Phòng thí nghiệm
}

/// <summary>
/// Trạng thái phòng
/// </summary>
public enum RoomStatus
{
    Available,   // Trống — có thể mượn
    InUse,       // Đang sử dụng
    Maintenance  // Đang bảo trì
}

/// <summary>
/// Loại yêu cầu mượn
/// </summary>
public enum RequestType
{
    InClass,     // Mượn trong giờ học (SV/GV)
    OffHours     // Mượn ngoài giờ (Đoàn/CLB/cá nhân)
}

/// <summary>
/// Trạng thái đơn mượn — vòng đời của 1 đơn
/// pending → approved/rejected → checked_out → returned
///                              → overdue (nếu trễ)
/// pending → cancelled (nếu hủy trước khi duyệt)
/// </summary>
public enum RequestStatus
{
    Pending,     // Chờ duyệt
    Approved,    // Đã duyệt (chờ QL bàn giao)
    Rejected,    // Bị từ chối
    CheckedOut,  // Đã bàn giao CSVC
    Returned,    // Đã hoàn trả
    Cancelled,   // Đã hủy
    Overdue      // Quá hạn trả
}

/// <summary>
/// Mức độ ưu tiên đơn mượn
/// </summary>
public enum PriorityLevel
{
    Low,         // Thấp
    Normal,      // Bình thường
    High         // Cao (sự kiện quan trọng)
}

/// <summary>
/// Mức độ nghiêm trọng sự cố
/// </summary>
public enum Severity
{
    Low,         // Nhẹ
    Medium,      // Trung bình
    High         // Nghiêm trọng
}

/// <summary>
/// Loại sự cố CSVC
/// </summary>
public enum IncidentType
{
    Damage,      // Hư hỏng
    Loss         // Thất lạc/Mất
}

/// <summary>
/// Trạng thái xử lý biên bản sự cố
/// </summary>
public enum DamageReportStatus
{
    Open,        // Mới mở — chưa xử lý
    Confirmed,   // Đã xác nhận — đang xử lý
    Closed       // Đã đóng — xử lý xong
}

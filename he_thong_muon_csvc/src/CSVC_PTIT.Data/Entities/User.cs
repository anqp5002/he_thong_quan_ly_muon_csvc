namespace CSVC_PTIT.Data.Entities;

using CSVC_PTIT.Data.Enums;

/// <summary>
/// Bảng 3: Người dùng hệ thống
/// Bao gồm tất cả: SV, GV, Bí thư Đoàn, QL_CSVC, Admin
/// </summary>
public class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    /// <summary>Mật khẩu đã được mã hóa bằng BCrypt</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Email bắt buộc, dùng để xác thực và phân quyền.
    /// SV: n23dccnxxx@student.ptithcm.edu.vn
    /// GV/NV: ten@ptithcm.edu.vn
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Đã xác nhận email chưa (qua link verify gửi về Gmail)</summary>
    public bool IsEmailVerified { get; set; } = false;
    public string? Phone { get; set; }
    public string FullName { get; set; } = string.Empty;

    /// <summary>Loại người mượn: Student, Lecturer, Club, Unit...</summary>
    public BorrowerType BorrowerType { get; set; }

    /// <summary>Mã số sinh viên (nếu là SV): N21DCCN001</summary>
    public string? StudentCode { get; set; }

    /// <summary>Mã nhân viên (nếu là GV/NV)</summary>
    public string? EmployeeCode { get; set; }

    /// <summary>Lớp (nếu là SV): D21CQCN01-N</summary>
    public string? ClassName { get; set; }

    /// <summary>CMND/CCCD</summary>
    public string? IdentityNo { get; set; }

    /// <summary>Trạng thái tài khoản</summary>
    public UserStatus Status { get; set; } = UserStatus.Active;

    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>Xóa mềm: true = đã xóa (không hiện trên giao diện)</summary>
    public bool IsDeleted { get; set; } = false;

    // ===== FK: Liên kết đến bảng khác =====

    /// <summary>FK → Bảng roles (SV/DT/QL/AD)</summary>
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    /// <summary>FK → Bảng departments (phòng ban/đơn vị)</summary>
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // ===== Navigation ngược: Các bảng khác trỏ về User =====

    /// <summary>Các đơn mượn do user này tạo</summary>
    public ICollection<BorrowRequest> BorrowRequests { get; set; } = new List<BorrowRequest>();

    /// <summary>Các đơn mượn mà user này duyệt (vai trò QL_CSVC)</summary>
    public ICollection<BorrowRequest> ApprovedRequests { get; set; } = new List<BorrowRequest>();
}

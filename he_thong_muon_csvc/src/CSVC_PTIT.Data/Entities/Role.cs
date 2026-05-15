namespace CSVC_PTIT.Data.Entities;

using CSVC_PTIT.Data.Enums;

/// <summary>
/// Bảng 1: Vai trò trong hệ thống
/// Dữ liệu cố định: SV, DT, QL, AD
/// </summary>
public class Role
{
    public int RoleId { get; set; }

    /// <summary>Mã vai trò: SV, DT, QL, AD</summary>
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>Tên đầy đủ: Sinh viên, Bí thư Đoàn, Quản lý CSVC, Admin</summary>
    public string RoleName { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation: 1 Role có nhiều Users
    public ICollection<User> Users { get; set; } = new List<User>();
}

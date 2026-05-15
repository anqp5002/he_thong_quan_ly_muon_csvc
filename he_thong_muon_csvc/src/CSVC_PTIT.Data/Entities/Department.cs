namespace CSVC_PTIT.Data.Entities;

/// <summary>
/// Bảng 2: Phòng ban / Đơn vị
/// VD: Khoa CNTT, LCĐ ATTT, CLB Guitar, Phòng TCHC-QT
/// </summary>
public class Department
{
    public int DepartmentId { get; set; }

    /// <summary>Mã đơn vị: CNTT, ATTT, CLB-GT...</summary>
    public string DepartmentCode { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation: 1 Department có nhiều Users
    public ICollection<User> Users { get; set; } = new List<User>();
}

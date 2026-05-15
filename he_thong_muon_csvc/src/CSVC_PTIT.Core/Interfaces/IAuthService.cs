using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

/// <summary>
/// Hợp đồng cho dịch vụ xác thực (đăng nhập/đăng xuất).
/// Sprint 1 — Task A.1 sẽ code logic thật.
/// </summary>
public interface IAuthService
{
    /// <summary>Đăng nhập bằng email + password</summary>
    Task<User?> LoginAsync(string email, string password);

    /// <summary>Đăng xuất — xóa session</summary>
    void Logout();

    /// <summary>User đang đăng nhập hiện tại (null = chưa login)</summary>
    User? CurrentUser { get; }

    /// <summary>Kiểm tra đã đăng nhập chưa</summary>
    bool IsLoggedIn { get; }
}

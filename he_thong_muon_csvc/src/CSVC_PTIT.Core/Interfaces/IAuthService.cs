using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

/// <summary>
/// Hợp đồng cho dịch vụ xác thực (đăng nhập/đăng xuất).
/// Sprint 1 — Task A.1
/// </summary>
public interface IAuthService
{
    /// <summary>Đăng nhập bằng email + password</summary>
    Task<User?> LoginAsync(string email, string password);

    /// <summary>Đổi mật khẩu</summary>
    Task ChangePasswordAsync(int userId, string oldPassword, string newPassword);

    /// <summary>Gửi mã OTP qua email để quên mật khẩu</summary>
    Task SendForgotPasswordOtpAsync(string email);

    /// <summary>Đặt lại mật khẩu bằng mã OTP</summary>
    Task ResetPasswordWithOtpAsync(string email, string otp, string newPassword);

    /// <summary>Đăng xuất — xóa session</summary>
    void Logout();

    /// <summary>Cập nhật thông tin profile</summary>
    Task UpdateProfileAsync(int userId, string fullName, string studentCode, string className, string phone);

    /// <summary>User đang đăng nhập hiện tại (null = chưa login)</summary>
    User? CurrentUser { get; }

    /// <summary>Kiểm tra đã đăng nhập chưa</summary>
    bool IsLoggedIn { get; }

    /// <summary>Số lần đăng nhập sai</summary>
    int FailedLoginAttempts { get; }

    /// <summary>Thời điểm hết hạn khóa tài khoản tạm thời</summary>
    DateTime? LockoutEndTime { get; }
}

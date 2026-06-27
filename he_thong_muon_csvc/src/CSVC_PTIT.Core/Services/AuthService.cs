using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSVC_PTIT.Core.Services;

/// <summary>
/// Service xác thực — Singleton
/// Sprint 1 — Task A.1
/// </summary>
public class AuthService : IAuthService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AuthService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;
    public int FailedLoginAttempts { get; private set; } = 0;
    public DateTime? LockoutEndTime { get; private set; }

    // Lưu trữ OTP tạm thời trong RAM. Cấu trúc: Email -> (OTP, ExpiryTime)
    private static readonly Dictionary<string, (string otp, DateTime expires)> _otpCache = new();

    public async Task<User?> LoginAsync(string email, string password)
    {
        if (LockoutEndTime.HasValue && LockoutEndTime.Value > DateTime.Now)
        {
            throw new Exception($"Bạn đã bị khóa. Vui lòng thử lại sau {(int)(LockoutEndTime.Value - DateTime.Now).TotalSeconds} giây.");
        }

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CsvcDbContext>();

        var user = await context.Users
            .Include(u => u.Role)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            if (email.EndsWith("@student.ptithcm.edu.vn") || email.EndsWith("@ptithcm.edu.vn"))
            {
                user = await AutoProvisionUserAsync(context, email);
            }
        }

        if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash))
        {
            FailedLoginAttempts++;
            if (FailedLoginAttempts >= 5)
            {
                LockoutEndTime = DateTime.Now.AddSeconds(30);
                throw new Exception("Bạn đã đăng nhập sai 5 lần. Khóa tạm 30 giây.");
            }
            return null; // Sai email hoặc password
        }

        // Đăng nhập thành công
        FailedLoginAttempts = 0;
        LockoutEndTime = null;
        user.LastLoginAt = DateTime.Now;
        await context.SaveChangesAsync();
        
        CurrentUser = user;

        var auditService = scope.ServiceProvider.GetRequiredService<IAuditLogService>();
        await auditService.LogAsync(user.UserId, "Đăng nhập", "Hệ thống", null, "Người dùng đăng nhập thành công");

        // Bắt buộc đổi mật khẩu nếu pass là 123456
        if (BCrypt.Net.BCrypt.EnhancedVerify("123456", user.PasswordHash))
        {
            throw new Exception("MUST_CHANGE_PASSWORD");
        }
        return user;
    }

    public async Task ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CsvcDbContext>();

        var user = await context.Users.FindAsync(userId);
        if (user == null)
            throw new Exception("Không tìm thấy người dùng");

        if (!BCrypt.Net.BCrypt.EnhancedVerify(oldPassword, user.PasswordHash))
            throw new Exception("Mật khẩu cũ không chính xác");

        user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
        await context.SaveChangesAsync();
    }

    private async Task<User> AutoProvisionUserAsync(CsvcDbContext context, string email)
    {
        // Sinh viên (Role 1), GV (Role 1 nhưng tuỳ chỉnh, tạm lấy Role SV)
        var role = await context.Roles.FirstOrDefaultAsync(r => r.RoleCode == "SV");
        int roleId = role?.RoleId ?? 1;

        var newUser = new User
        {
            Username = email.Split('@')[0],
            Email = email,
            FullName = "Người dùng mới",
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("123456"),
            RoleId = roleId,
            BorrowerType = CSVC_PTIT.Data.Enums.BorrowerType.Student,
            IsEmailVerified = false,
            StudentCode = email.Split('@')[0].ToUpper(),
            CreatedAt = DateTime.Now
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        // Nạp lại Role
        await context.Entry(newUser).Reference(u => u.Role).LoadAsync();
        return newUser;
    }

    public async Task SendForgotPasswordOtpAsync(string email)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CsvcDbContext>();
        
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            throw new Exception("Không tìm thấy tài khoản với email này.");

        // Sinh OTP ngẫu nhiên 6 số
        var otp = new Random().Next(100000, 999999).ToString();
        var expires = DateTime.Now.AddMinutes(5);

        _otpCache[email] = (otp, expires);

        // Gửi mail
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        string subject = "Mã xác nhận quên mật khẩu - CSVC PTIT";
        string body = $"Chào {user.FullName},\n\nMã xác nhận (OTP) để đổi mật khẩu của bạn là: {otp}\n\nMã này sẽ hết hạn sau 5 phút.\n\nTrân trọng,\nHệ thống quản lý CSVC PTIT.";
        
        await emailService.SendEmailAsync(email, subject, body);
    }

    public async Task ResetPasswordWithOtpAsync(string email, string otp, string newPassword)
    {
        if (!_otpCache.TryGetValue(email, out var cache) || cache.otp != otp)
            throw new Exception("Mã xác nhận không đúng.");

        if (DateTime.Now > cache.expires)
        {
            _otpCache.Remove(email);
            throw new Exception("Mã xác nhận đã hết hạn.");
        }

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CsvcDbContext>();

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            throw new Exception("Không tìm thấy người dùng.");

        user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
        await context.SaveChangesAsync();

        // Xóa OTP
        _otpCache.Remove(email);
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}

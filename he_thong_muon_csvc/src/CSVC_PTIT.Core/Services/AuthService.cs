using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
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
            .FirstOrDefaultAsync(u => u.Email == email);

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

    public void Logout()
    {
        CurrentUser = null;
    }
}

using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

/// <summary>
/// Hợp đồng cho dịch vụ quản lý tài khoản (UC-AD01).
/// Sprint 1 — Task A.4 sẽ code logic thật.
/// </summary>
public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int userId);
    Task<User> CreateAsync(User user, string password);
    Task UpdateAsync(User user);
    Task LockAsync(int userId);
    Task UnlockAsync(int userId);
}

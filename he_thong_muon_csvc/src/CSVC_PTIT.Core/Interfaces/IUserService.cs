using CSVC_PTIT.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSVC_PTIT.Core.Interfaces;

/// <summary>
/// Hợp đồng cho dịch vụ quản lý tài khoản (UC-AD01).
/// Sprint 1 — Task A.4
/// </summary>
public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int userId);
    Task<User> CreateAsync(User user, string password);
    Task UpdateAsync(User user);
    Task LockAsync(int userId);
    Task UnlockAsync(int userId);
    Task DeleteAsync(int userId); // Soft delete
    Task AssignRoleAsync(int userId, int roleId);
    Task<List<Role>> GetRolesAsync();
    Task<List<Department>> GetDepartmentsAsync();
}

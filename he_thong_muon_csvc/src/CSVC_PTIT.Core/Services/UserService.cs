using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSVC_PTIT.Core.Services;

/// <summary>
/// Service quản lý tài khoản
/// Sprint 1 — Task A.4
/// </summary>
public class UserService : IUserService
{
    private readonly CsvcDbContext _context;

    public UserService(CsvcDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Department)
            .Where(u => !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
    }

    public async Task<User> CreateAsync(User user, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == user.Email && !u.IsDeleted))
            throw new Exception("Email này đã được sử dụng.");

        user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        user.CreatedAt = DateTime.Now;
        user.UpdatedAt = DateTime.Now;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        var existing = await _context.Users.FindAsync(user.UserId);
        if (existing == null || existing.IsDeleted)
            throw new Exception("Không tìm thấy tài khoản.");

        if (existing.Email != user.Email && await _context.Users.AnyAsync(u => u.Email == user.Email && !u.IsDeleted))
            throw new Exception("Email này đã được sử dụng.");

        existing.FullName = user.FullName;
        existing.Email = user.Email;
        existing.Phone = user.Phone;
        existing.RoleId = user.RoleId;
        existing.DepartmentId = user.DepartmentId;
        existing.BorrowerType = user.BorrowerType;
        existing.StudentCode = user.StudentCode;
        existing.EmployeeCode = user.EmployeeCode;
        existing.ClassName = user.ClassName;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task LockAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Status = UserStatus.Locked;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnlockAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Status = UserStatus.Active;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AssignRoleAsync(int userId, int roleId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.IsDeleted)
            throw new Exception("Không tìm thấy tài khoản.");
        user.RoleId = roleId;
        user.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Role>> GetRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        return await _context.Departments.ToListAsync();
    }
}

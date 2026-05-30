using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSVC_PTIT.Core.Services;

/// <summary>
/// Service ghi và truy vấn nhật ký hoạt động.
/// Sprint 1 — Task A.9
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly CsvcDbContext _context;

    public AuditLogService(CsvcDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(int userId, string action, string targetType, int? targetId, string? detail)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            TargetType = targetType,
            TargetId = targetId,
            Detail = detail,
            CreatedAt = DateTime.Now
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetLogsAsync(DateTime? from, DateTime? to, int? userId, string? action)
    {
        var query = _context.AuditLogs
            .Include(l => l.User)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(l => l.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(l => l.CreatedAt <= to.Value.AddDays(1));

        if (userId.HasValue)
            query = query.Where(l => l.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(l => l.Action == action);

        return await query
            .OrderByDescending(l => l.CreatedAt)
            .Take(500) // Giới hạn để không quá nặng
            .ToListAsync();
    }
}

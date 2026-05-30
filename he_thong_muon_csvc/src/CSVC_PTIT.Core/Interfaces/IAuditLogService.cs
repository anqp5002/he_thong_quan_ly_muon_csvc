using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

/// <summary>
/// Hợp đồng cho dịch vụ ghi và truy vấn nhật ký hoạt động (UC-AD04).
/// Sprint 1 — Task A.9
/// </summary>
public interface IAuditLogService
{
    /// <summary>Ghi 1 dòng nhật ký hoạt động</summary>
    Task LogAsync(int userId, string action, string targetType, int? targetId, string? detail);

    /// <summary>Truy vấn nhật ký có filter</summary>
    Task<List<AuditLog>> GetLogsAsync(DateTime? from, DateTime? to, int? userId, string? action);
}

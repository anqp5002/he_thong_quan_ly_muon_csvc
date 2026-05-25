using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSVC_PTIT.Data.Entities;

/// <summary>
/// Lưu trữ lịch sử hoạt động của người dùng (Audit Log) cho mục đích theo dõi và bảo mật.
/// </summary>
public class AuditLog
{
    [Key]
    public int LogId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(100)]
    public string TargetType { get; set; } = string.Empty;

    public int? TargetId { get; set; }

    public string? Detail { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}

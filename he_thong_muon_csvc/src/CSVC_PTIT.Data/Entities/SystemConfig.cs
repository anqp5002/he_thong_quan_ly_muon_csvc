using System;
using System.ComponentModel.DataAnnotations;

namespace CSVC_PTIT.Data.Entities;

/// <summary>
/// Lưu trữ các cấu hình hệ thống có thể thay đổi bởi Admin (ví dụ: số giờ mượn tối đa, bật/tắt tính năng).
/// </summary>
public class SystemConfig
{
    [Key]
    public int ConfigId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ConfigKey { get; set; } = string.Empty;

    [Required]
    public string ConfigValue { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int? UpdatedBy { get; set; }
}

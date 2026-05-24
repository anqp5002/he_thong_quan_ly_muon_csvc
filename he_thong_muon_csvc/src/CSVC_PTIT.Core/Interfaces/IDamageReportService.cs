using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;

namespace CSVC_PTIT.Core.Interfaces;

public class DamageReportDto
{
    public int RequestId { get; set; }
    public int? ReturnItemId { get; set; }
    public int AssetId { get; set; }
    public int ResponsibleUserId { get; set; }
    public Severity Severity { get; set; }
    public IncidentType IncidentType { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal EstimatedCompensation { get; set; }
}

public interface IDamageReportService
{
    Task<DamageReport> CreateReportAsync(int reportedByUserId, DamageReportDto dto);
    Task<IEnumerable<DamageReport>> GetAllReportsAsync();
    Task<DamageReport?> GetReportByIdAsync(int reportId);
    Task ResolveReportAsync(int reportId, decimal actualCompensation, string note);
}

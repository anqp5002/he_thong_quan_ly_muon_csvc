using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CSVC_PTIT.Core.Services;

public class DamageReportService : IDamageReportService
{
    private readonly CsvcDbContext _context;

    public DamageReportService(CsvcDbContext context)
    {
        _context = context;
    }

    public async Task<DamageReport> CreateReportAsync(int reportedByUserId, DamageReportDto dto)
    {
        var asset = await _context.Assets.FindAsync(dto.AssetId);
        if (asset == null) throw new Exception("Không tìm thấy CSVC.");

        var report = new DamageReport
        {
            ReportedBy = reportedByUserId,
            RequestId = dto.RequestId,
            ReturnItemId = dto.ReturnItemId,
            AssetId = dto.AssetId,
            ResponsibleUserId = dto.ResponsibleUserId,
            Severity = dto.Severity,
            IncidentType = dto.IncidentType,
            Description = dto.Description,
            EstimatedCompensation = dto.EstimatedCompensation,
            ReportedAt = DateTime.Now,
            Status = DamageReportStatus.Open
        };

        // Cập nhật tình trạng vật lý + giảm số lượng khả dụng
        if (dto.IncidentType == IncidentType.Damage)
        {
            asset.ConditionStatus = ConditionStatus.Damaged;
            // Giảm số lượng khả dụng (đồ hỏng không thể cho mượn tiếp)
            if (asset.AvailableQuantity > 0)
                asset.AvailableQuantity--;
        }
        else if (dto.IncidentType == IncidentType.Loss)
        {
            asset.ConditionStatus = ConditionStatus.Damaged;
            asset.AvailabilityStatus = AvailabilityStatus.Unavailable;
            // Giảm cả tổng lẫn khả dụng (đồ bị mất hoàn toàn)
            if (asset.TotalQuantity > 0) asset.TotalQuantity--;
            if (asset.AvailableQuantity > 0) asset.AvailableQuantity--;
        }

        _context.DamageReports.Add(report);
        await _context.SaveChangesAsync();

        return report;
    }

    public async Task<IEnumerable<DamageReport>> GetAllReportsAsync()
    {
        return await _context.DamageReports
            .Include(r => r.Asset)
            .Include(r => r.ResponsibleUser)
            .Include(r => r.ReportedByUser)
            .Include(r => r.BorrowRequest)
            .ToListAsync();
    }

    public async Task<DamageReport?> GetReportByIdAsync(int reportId)
    {
        return await _context.DamageReports
            .Include(r => r.Asset)
            .Include(r => r.ResponsibleUser)
            .Include(r => r.ReportedByUser)
            .Include(r => r.BorrowRequest)
            .FirstOrDefaultAsync(r => r.ReportId == reportId);
    }

    public async Task ResolveReportAsync(int reportId, decimal actualCompensation, string note)
    {
        var report = await _context.DamageReports.FindAsync(reportId);
        if (report == null) throw new Exception("Không tìm thấy biên bản.");

        report.Status = DamageReportStatus.Closed;
        report.ActualCompensation = actualCompensation;
        report.ResolutionNote = note;
        report.ResolvedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }
}

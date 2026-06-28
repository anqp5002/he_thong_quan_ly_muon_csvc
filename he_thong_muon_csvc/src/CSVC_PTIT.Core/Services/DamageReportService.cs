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

        var responsibleUser = await _context.Users.FindAsync(dto.ResponsibleUserId);
        if (responsibleUser == null) throw new Exception("Không tìm thấy người chịu trách nhiệm.");

        var requestStatus = await _context.BorrowRequests
            .Where(r => r.RequestId == dto.RequestId)
            .Select(r => (RequestStatus?)r.Status)
            .FirstOrDefaultAsync();
        if (!requestStatus.HasValue) throw new Exception("Không tìm thấy đơn mượn liên quan.");

        var returnItemId = dto.ReturnItemId;
        if (!returnItemId.HasValue)
        {
            returnItemId = await _context.ReturnItems
                .Where(ri => ri.AssetId == dto.AssetId && (ri.IsDamaged || ri.IsLost))
                .Where(ri => ri.Return.Checkout.RequestId == dto.RequestId)
                .OrderByDescending(ri => ri.Return.ReturnedAt)
                .Select(ri => (int?)ri.ReturnItemId)
                .FirstOrDefaultAsync();
        }

        if (returnItemId.HasValue)
        {
            var returnItemExists = await _context.ReturnItems.AnyAsync(ri => ri.ReturnItemId == returnItemId.Value);
            if (!returnItemExists) throw new Exception("Không tìm thấy dòng trả có sự cố.");
        }

        var report = new DamageReport
        {
            ReportedBy = reportedByUserId,
            RequestId = dto.RequestId,
            ReturnItemId = returnItemId,
            AssetId = dto.AssetId,
            ResponsibleUserId = dto.ResponsibleUserId,
            Severity = dto.Severity,
            IncidentType = dto.IncidentType,
            Description = dto.Description,
            EstimatedCompensation = dto.EstimatedCompensation,
            ReportedAt = DateTime.Now,
            Status = DamageReportStatus.Open
        };

        var assetAlreadyReservedByRequest =
            requestStatus == RequestStatus.Approved ||
            requestStatus == RequestStatus.CheckedOut;

        if (returnItemId.HasValue || assetAlreadyReservedByRequest)
        {
            asset.ConditionStatus = ConditionStatus.Damaged;
            if (dto.IncidentType == IncidentType.Loss)
                asset.AvailabilityStatus = AvailabilityStatus.Unavailable;
        }
        else if (dto.IncidentType == IncidentType.Damage)
        {
            asset.ConditionStatus = ConditionStatus.Damaged;
            if (asset.AvailableQuantity > 0)
                asset.AvailableQuantity--;
        }
        else if (dto.IncidentType == IncidentType.Loss)
        {
            asset.ConditionStatus = ConditionStatus.Damaged;
            asset.AvailabilityStatus = AvailabilityStatus.Unavailable;
            if (asset.TotalQuantity > 0) asset.TotalQuantity--;
            if (asset.AvailableQuantity > 0) asset.AvailableQuantity--;
        }

        responsibleUser.Status = UserStatus.Locked;

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
                .ThenInclude(br => br.Checkouts)
                    .ThenInclude(c => c.CheckoutItems)
            .FirstOrDefaultAsync(r => r.ReportId == reportId);
    }

    public async Task ResolveReportAsync(int reportId, decimal actualCompensation, string note)
    {
        var report = await _context.DamageReports
            .Include(r => r.ResponsibleUser)
            .Include(r => r.BorrowRequest)
                .ThenInclude(br => br.Checkouts)
                    .ThenInclude(c => c.CheckoutItems)
            .FirstOrDefaultAsync(r => r.ReportId == reportId);
        if (report == null) throw new Exception("Không tìm thấy biên bản.");

        report.Status = DamageReportStatus.Closed;
        report.ActualCompensation = actualCompensation;
        report.ResolutionNote = note;
        report.ResolvedAt = DateTime.Now;

        if (report.BorrowRequest != null &&
            report.BorrowRequest.Status == RequestStatus.CheckedOut &&
            report.BorrowRequest.Checkouts.SelectMany(c => c.CheckoutItems).All(ci => ci.IsReturned))
        {
            report.BorrowRequest.Status = RequestStatus.Returned;
            report.BorrowRequest.ActualReturnAt = DateTime.Now;
        }

        var hasOpenReports = await _context.DamageReports
            .AnyAsync(dr => dr.ResponsibleUserId == report.ResponsibleUserId
                && dr.ReportId != reportId
                && dr.Status != DamageReportStatus.Closed);

        if (!hasOpenReports &&
            report.ResponsibleUser != null &&
            report.ResponsibleUser.Status == UserStatus.Locked)
        {
            report.ResponsibleUser.Status = UserStatus.Active;
        }

        await _context.SaveChangesAsync();
    }
}

using CSVC_PTIT.Core.DTOs;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CSVC_PTIT.Core.Services;

public class BorrowService : IBorrowService
{
    private readonly CsvcDbContext _context;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;

    public BorrowService(
        CsvcDbContext context,
        IEmailService emailService,
        INotificationService notificationService)
    {
        _context = context;
        _emailService = emailService;
        _notificationService = notificationService;
    }

    public async Task<BorrowRequest> CreateInClassRequestAsync(CreateBorrowRequestDto dto)
    {
        // Sinh mã đơn tự động: SV-001, SV-002...
        var count = await _context.BorrowRequests.CountAsync() + 1;
        var requestCode = $"SV-{count:D3}";

        var request = new BorrowRequest
        {
            RequestCode = requestCode,
            RequestType = RequestType.InClass,
            RequesterId = dto.RequesterId,
            ContactPhone = dto.ContactPhone,
            Title = dto.Title,
            Purpose = dto.Purpose,
            RequestNote = dto.RequestNote,
            BorrowStartAt = dto.BorrowStartAt,
            BorrowEndAt = dto.BorrowEndAt,
            ExpectedReturnAt = dto.BorrowEndAt,
            Status = RequestStatus.Pending,
            PriorityLevel = PriorityLevel.Normal,
            CreatedAt = DateTime.Now
        };

        // Tạo các dòng CSVC trong đơn
        foreach (var item in dto.Assets)
        {
            request.BorrowRequestAssets.Add(new BorrowRequestAsset
            {
                AssetId = item.AssetId,
                QuantityRequested = item.QuantityRequested,
                QuantityApproved = 0,
                QuantityCheckedOut = 0,
                QuantityReturned = 0,
                ItemNote = item.ItemNote
            });
        }

        _context.BorrowRequests.Add(request);
        await _context.SaveChangesAsync();

        await NotifyManagersAsync(request);

        return request;
    }

    private async Task NotifyManagersAsync(BorrowRequest request)
    {
        var managers = await _context.Users
            .Include(u => u.Role)
            .Where(u => u.Role.RoleCode == "QL")
            .ToListAsync();

        string title = $"Đơn mượn mới: {request.RequestCode}";
        string message = $"Có một đơn mượn CSVC mới ({request.RequestType}) cần được duyệt.";

        foreach (var manager in managers)
        {
            await _notificationService.CreateNotificationAsync(manager.UserId, title, message);
            await _emailService.SendEmailAsync(manager.Email, title, message);
        }
    }

    public async Task<List<BorrowRequest>> GetRequestsByUserAsync(int userId)
    {
        return await _context.BorrowRequests
            .Include(r => r.BorrowRequestAssets)
                .ThenInclude(a => a.Asset)
            .Where(r => r.RequesterId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
    public async Task<BorrowRequest> CreateOffHoursRequestAsync(CreateBorrowRequestDto dto)
    {
        // Sinh mã đơn DT-001, DT-002...
        var count = await _context.BorrowRequests
            .Where(r => r.RequestType == RequestType.OffHours)
            .CountAsync() + 1;
        var requestCode = $"DT-{count:D3}";

        var request = new BorrowRequest
        {
            RequestCode = requestCode,
            RequestType = RequestType.OffHours,
            RequesterId = dto.RequesterId,
            ContactPhone = dto.ContactPhone,
            Title = dto.Title,
            Purpose = dto.Purpose,
            RequestNote = dto.RequestNote,
            BorrowStartAt = dto.BorrowStartAt,
            BorrowEndAt = dto.BorrowEndAt,
            ExpectedReturnAt = dto.BorrowEndAt,
            Status = RequestStatus.Pending,
            PriorityLevel = PriorityLevel.Normal,
            CreatedAt = DateTime.Now
        };

        foreach (var item in dto.Assets)
        {
            request.BorrowRequestAssets.Add(new BorrowRequestAsset
            {
                AssetId = item.AssetId,
                QuantityRequested = item.QuantityRequested,
                QuantityApproved = 0,
                QuantityCheckedOut = 0,
                QuantityReturned = 0,
                ItemNote = item.ItemNote
            });
        }

        _context.BorrowRequests.Add(request);
        await _context.SaveChangesAsync();

        await NotifyManagersAsync(request);

        return request;
    }

    public async Task<List<BorrowRequest>> GetRequestsByStatusAsync(RequestStatus status)
    {
        return await _context.BorrowRequests
            .Include(r => r.Requester)
            .Include(r => r.BorrowRequestAssets)
                .ThenInclude(a => a.Asset)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task ApproveRequestAsync(int requestId, int approverId)
    {
        var request = await _context.BorrowRequests
            .Include(r => r.BorrowRequestAssets)
            .FirstOrDefaultAsync(r => r.RequestId == requestId);

        if (request == null)
            throw new Exception("Không tìm thấy đơn mượn.");

        if (request.Status != RequestStatus.Pending)
            throw new Exception("Đơn mượn không ở trạng thái chờ duyệt.");

        request.Status = RequestStatus.Approved;
        request.ApprovedBy = approverId;
        request.ApprovedAt = DateTime.Now;

        // Tự động gán số lượng duyệt = số lượng yêu cầu
        foreach (var item in request.BorrowRequestAssets)
        {
            item.QuantityApproved = item.QuantityRequested;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RejectRequestAsync(int requestId, int approverId, string reason)
    {
        var request = await _context.BorrowRequests.FindAsync(requestId);

        if (request == null)
            throw new Exception("Không tìm thấy đơn mượn.");

        if (request.Status != RequestStatus.Pending)
            throw new Exception("Đơn mượn không ở trạng thái chờ duyệt.");

        request.Status = RequestStatus.Rejected;
        request.ApprovedBy = approverId;
        request.ApprovedAt = DateTime.Now;
        request.RejectReason = reason;

        await _context.SaveChangesAsync();
    }
}
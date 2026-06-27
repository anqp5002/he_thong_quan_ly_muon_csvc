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
    private readonly IAuditLogService _auditLogService;

    public BorrowService(
        CsvcDbContext context,
        IEmailService emailService,
        INotificationService notificationService,
        IAuditLogService auditLogService)
    {
        _context = context;
        _emailService = emailService;
        _notificationService = notificationService;
        _auditLogService = auditLogService;
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

        if (dto.RoomId.HasValue)
        {
            request.BorrowRequestRooms.Add(new BorrowRequestRoom
            {
                RoomId = dto.RoomId.Value
            });
        }

        _context.BorrowRequests.Add(request);
        await _context.SaveChangesAsync();

        // Reload đầy đủ thông tin Requester + Department + Assets để xuất PDF không bị thiếu
        var fullRequest = await _context.BorrowRequests
            .Include(r => r.Requester)
                .ThenInclude(u => u.Department)
            .Include(r => r.BorrowRequestAssets)
                .ThenInclude(ra => ra.Asset)
            .FirstAsync(r => r.RequestId == request.RequestId);

        await NotifyManagersAsync(fullRequest);

        return fullRequest;
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
            .AsNoTracking()
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

        if (dto.RoomId.HasValue)
        {
            request.BorrowRequestRooms.Add(new BorrowRequestRoom
            {
                RoomId = dto.RoomId.Value
            });
        }

        _context.BorrowRequests.Add(request);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(dto.RequesterId, "Tạo đơn", "BorrowRequest", request.RequestId, $"Đoàn thể tạo đơn mượn ngoài giờ {request.RequestCode}");

        // Reload đầy đủ thông tin
        var fullRequest = await _context.BorrowRequests
            .Include(r => r.Requester)
                .ThenInclude(u => u.Department)
            .Include(r => r.BorrowRequestAssets)
                .ThenInclude(ra => ra.Asset)
            .FirstAsync(r => r.RequestId == request.RequestId);

        await NotifyManagersAsync(fullRequest);

        return fullRequest;
    }

    public async Task<List<BorrowRequest>> GetRequestsByStatusAsync(RequestStatus status)
    {
        return await _context.BorrowRequests
            .AsNoTracking()
            .Include(r => r.Requester)
            .Include(r => r.BorrowRequestAssets)
                .ThenInclude(a => a.Asset)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task ApproveRequestAsync(int requestId, int approverId)
    {
        _context.ChangeTracker.Clear();

        var request = await _context.BorrowRequests
            .Include(r => r.BorrowRequestAssets)
                .ThenInclude(ra => ra.Asset)
            .Include(r => r.Requester)
            .Include(r => r.BorrowRequestRooms)
            .FirstOrDefaultAsync(r => r.RequestId == requestId);

        if (request == null)
            throw new Exception("Không tìm thấy đơn mượn.");

        if (request.Status != RequestStatus.Pending)
            throw new Exception("Đơn mượn không ở trạng thái chờ duyệt.");

        // Kiểm tra xung đột Phòng
        if (request.BorrowRequestRooms.Any())
        {
            var roomIds = request.BorrowRequestRooms.Select(rr => rr.RoomId).ToList();
            var conflictingRequests = await _context.BorrowRequests
                .Include(r => r.BorrowRequestRooms)
                    .ThenInclude(rr => rr.Room)
                .Where(r => r.RequestId != requestId 
                    && (r.Status == RequestStatus.Approved || r.Status == RequestStatus.CheckedOut)
                    && r.BorrowStartAt < request.BorrowEndAt
                    && r.BorrowEndAt > request.BorrowStartAt
                    && r.BorrowRequestRooms.Any(rr => roomIds.Contains(rr.RoomId)))
                .ToListAsync();

            if (conflictingRequests.Any())
            {
                var conflict = conflictingRequests.First();
                var roomName = conflict.BorrowRequestRooms.FirstOrDefault(rr => roomIds.Contains(rr.RoomId))?.Room?.RoomCode ?? "Phòng";
                throw new Exception($"⚠️ XUNG ĐỘT PHÒNG: {roomName} đã được cấp cho đơn {conflict.RequestCode} từ {conflict.BorrowStartAt:HH:mm} đến {conflict.BorrowEndAt:HH:mm}. Vui lòng từ chối đơn hoặc yêu cầu sinh viên đổi phòng.");
            }
        }

        // Kiểm tra tồn kho trước khi duyệt
        var insufficientItems = new List<string>();
        foreach (var item in request.BorrowRequestAssets)
        {
            if (item.Asset != null && item.QuantityRequested > item.Asset.AvailableQuantity)
            {
                insufficientItems.Add($"{item.Asset.AssetName} (yêu cầu: {item.QuantityRequested}, khả dụng: {item.Asset.AvailableQuantity})");
            }
        }

        if (insufficientItems.Any())
        {
            throw new Exception($"Không đủ tồn kho để duyệt đơn:\n" + string.Join("\n", insufficientItems));
        }

        request.Status = RequestStatus.Approved;
        request.ApprovedBy = approverId;
        request.ApprovedAt = DateTime.Now;

        // Tự động gán số lượng duyệt = số lượng yêu cầu
        foreach (var item in request.BorrowRequestAssets)
        {
            item.QuantityApproved = item.QuantityRequested;
        }

        await _context.SaveChangesAsync();

        // Gửi thông báo cho sinh viên
        await _notificationService.CreateNotificationAsync(
            request.RequesterId,
            $"Đơn mượn {request.RequestCode} đã được duyệt",
            $"Đơn mượn của bạn đã được duyệt. Vui lòng đến phòng CSVC để nhận thiết bị.");

        await _auditLogService.LogAsync(approverId, "Duyệt", "BorrowRequest", requestId, $"Quản lý duyệt đơn mượn {request.RequestCode}");
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

        // Gửi thông báo cho sinh viên biết lý do từ chối
        await _notificationService.CreateNotificationAsync(
            request.RequesterId,
            $"Đơn mượn {request.RequestCode} bị từ chối",
            $"Đơn mượn của bạn đã bị từ chối. Lý do: {reason}");

        await _auditLogService.LogAsync(approverId, "Từ chối", "BorrowRequest", requestId, $"Quản lý từ chối đơn {request.RequestCode}: {reason}");
    }
}
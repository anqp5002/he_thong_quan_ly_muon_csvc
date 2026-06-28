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

    // Khung giờ cho phép sinh viên mượn (hard-code theo yêu cầu)
    private static readonly TimeSpan MornStart = new(7, 0, 0);
    private static readonly TimeSpan MornEnd = new(10, 30, 0);
    private static readonly TimeSpan AfterStart = new(13, 0, 0);
    private static readonly TimeSpan AfterEnd = new(16, 30, 0);

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

    /// <summary>
    /// Kiểm tra tài khoản có bị khóa không (BR-07).
    /// Nếu bị khóa do sự cố chưa xử lý → không cho tạo đơn.
    /// </summary>
    private async Task ValidateUserNotLocked(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("Không tìm thấy người dùng.");
        if (user.Status == UserStatus.Locked)
            throw new Exception("Tài khoản của bạn tạm thời bị khóa quyền mượn thiết bị do có sự cố chưa xử lý. Vui lòng liên hệ phòng CSVC để được hỗ trợ.");
    }

    /// <summary>
    /// Kiểm tra email sinh viên chỉ mượn trong khung giờ cho phép (BR-02).
    /// @student.ptithcm.edu.vn → Sáng 7:00-10:30, Chiều 13:00-16:30.
    /// </summary>
    private async Task ValidateStudentTimeSlot(int userId, DateTime startAt, DateTime endAt)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        // Chỉ kiểm tra với tài khoản sinh viên (@student)
        if (!user.Email.EndsWith("@student.ptithcm.edu.vn", StringComparison.OrdinalIgnoreCase))
            return;

        var startTime = startAt.TimeOfDay;
        var endTime = endAt.TimeOfDay;

        bool inMorning = startTime >= MornStart && endTime <= MornEnd;
        bool inAfternoon = startTime >= AfterStart && endTime <= AfterEnd;

        if (!inMorning && !inAfternoon)
        {
            throw new Exception(
                $"Sinh viên chỉ được mượn CSVC trong khung giờ:\n" +
                $"• Sáng: 07:00 – 10:30\n" +
                $"• Chiều: 13:00 – 16:30\n\n" +
                $"Thời gian bạn chọn ({startAt:HH:mm} – {endAt:HH:mm}) không hợp lệ.");
        }
    }

    private async Task ValidateOffHoursPermission(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("Không tìm thấy người dùng.");

        if (user.Email.EndsWith("@student.ptithcm.edu.vn", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception(
                "Tài khoản sinh viên không có quyền tạo đơn mượn ngoài giờ.\n" +
                "Vui lòng liên hệ Bí thư LCĐ hoặc Chủ nhiệm CLB để tạo đơn.");
        }
    }

    /// <summary>
    /// Kiểm tra trùng lịch phòng khi tạo đơn mượn.
    /// Nếu phòng đã được mượn (trạng thái Approved hoặc CheckedOut) trong khung giờ này thì từ chối.
    /// </summary>
    private async Task ValidateRoomNotOverlapped(int? roomId, DateTime startAt, DateTime endAt)
    {
        if (!roomId.HasValue) return;

        var conflictingRequests = await _context.BorrowRequests
            .Include(r => r.BorrowRequestRooms)
                .ThenInclude(rr => rr.Room)
            .Where(r => r.Status == RequestStatus.Approved || r.Status == RequestStatus.CheckedOut)
            .Where(r => r.BorrowStartAt < endAt && r.BorrowEndAt > startAt)
            .Where(r => r.BorrowRequestRooms.Any(rr => rr.RoomId == roomId.Value))
            .ToListAsync();

        if (conflictingRequests.Any())
        {
            var conflict = conflictingRequests.First();
            var roomName = conflict.BorrowRequestRooms.FirstOrDefault(rr => rr.RoomId == roomId.Value)?.Room?.RoomCode ?? "Phòng này";
            throw new Exception($"⚠️ Không thể mượn: {roomName} đã được sử dụng từ {conflict.BorrowStartAt:HH:mm} đến {conflict.BorrowEndAt:HH:mm} ngày {conflict.BorrowStartAt:dd/MM/yyyy}.\nVui lòng chọn phòng khác hoặc đổi khung giờ.");
        }
    }

    public async Task<BorrowRequest> CreateInClassRequestAsync(CreateBorrowRequestDto dto)
    {
        // BR-07: Kiểm tra tài khoản bị khóa
        await ValidateUserNotLocked(dto.RequesterId);

        // BR-02: Kiểm tra khung giờ sinh viên
        await ValidateStudentTimeSlot(dto.RequesterId, dto.BorrowStartAt, dto.BorrowEndAt);

        // Kiểm tra trùng lịch phòng
        await ValidateRoomNotOverlapped(dto.RoomId, dto.BorrowStartAt, dto.BorrowEndAt);

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
        // BR-07: Kiểm tra tài khoản bị khóa
        await ValidateUserNotLocked(dto.RequesterId);

        // Chặn sinh viên tạo đơn ngoài giờ
        await ValidateOffHoursPermission(dto.RequesterId);

        // Kiểm tra trùng lịch phòng
        await ValidateRoomNotOverlapped(dto.RoomId, dto.BorrowStartAt, dto.BorrowEndAt);

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
            AttachmentPath = dto.AttachmentPath,
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

        // Kiểm tra tồn kho VÀ trừ ngay khi duyệt (BR-03)
        foreach (var item in request.BorrowRequestAssets)
        {
            if (item.Asset != null && item.QuantityRequested > item.Asset.AvailableQuantity)
            {
                throw new Exception($"Không đủ tồn kho: {item.Asset.AssetName} (yêu cầu: {item.QuantityRequested}, khả dụng: {item.Asset.AvailableQuantity})");
            }
        }

        request.Status = RequestStatus.Approved;
        request.ApprovedBy = approverId;
        request.ApprovedAt = DateTime.Now;

        // Tự động gán số lượng duyệt = số lượng yêu cầu VÀ TRỪ TỒN KHO NGAY
        foreach (var item in request.BorrowRequestAssets)
        {
            item.QuantityApproved = item.QuantityRequested;

            // BR-03: Trừ số lượng khả dụng ngay khi duyệt
            if (item.Asset != null)
            {
                item.Asset.AvailableQuantity -= item.QuantityRequested;
            }
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
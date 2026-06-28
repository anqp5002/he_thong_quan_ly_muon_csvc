using CSVC_PTIT.Core.DTOs;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CSVC_PTIT.Core.Services;

public class CheckoutService : ICheckoutService
{
    private readonly CsvcDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public CheckoutService(CsvcDbContext context, IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    public async Task<Checkout> CreateCheckoutAsync(int requestId, int checkedOutByUserId, string note, Dictionary<int, string> assetConditions)
    {
        _context.ChangeTracker.Clear();

        var request = await _context.BorrowRequests
            .Include(r => r.BorrowRequestAssets)
            .ThenInclude(ra => ra.Asset)
            .Include(r => r.BorrowRequestRooms)
                .ThenInclude(rr => rr.Room)
            .FirstOrDefaultAsync(r => r.RequestId == requestId);

        if (request == null) throw new Exception("Không tìm thấy đơn mượn.");
        if (request.Status != RequestStatus.Approved) throw new Exception("Đơn mượn chưa được duyệt hoặc đã xử lý.");

        // Kiểm tra xung đột: CSVC đã được bàn giao cho đơn khác trong cùng khoảng thời gian
        var assetIds = request.BorrowRequestAssets.Select(ra => ra.AssetId).ToList();
        var conflictingCheckouts = await _context.Checkouts
            .Include(c => c.BorrowRequest)
            .Include(c => c.CheckoutItems)
                .ThenInclude(ci => ci.Asset)
            .Where(c => c.BorrowRequest.Status == RequestStatus.CheckedOut
                && c.CheckoutItems.Any(ci => assetIds.Contains(ci.AssetId) && !ci.IsReturned)
                && c.BorrowRequest.BorrowStartAt < request.BorrowEndAt
                && c.BorrowRequest.BorrowEndAt > request.BorrowStartAt)
            .ToListAsync();

        if (conflictingCheckouts.Any())
        {
            var warnings = new List<string>();
            foreach (var conflict in conflictingCheckouts)
            {
                var overlappingAssets = conflict.CheckoutItems
                    .Where(ci => assetIds.Contains(ci.AssetId) && !ci.IsReturned)
                    .Select(ci => ci.Asset?.AssetName ?? "?");
                warnings.Add($"Đơn {conflict.BorrowRequest.RequestCode} ({conflict.BorrowRequest.BorrowStartAt:dd/MM HH:mm} - {conflict.BorrowRequest.BorrowEndAt:dd/MM HH:mm}): {string.Join(", ", overlappingAssets)}");
            }
            throw new Exception($"⚠️ CẢNH BÁO XUNG ĐỘT: Các CSVC sau đang được mượn bởi đơn khác cùng thời gian:\n" + string.Join("\n", warnings) + "\n\nVui lòng kiểm tra lại trước khi bàn giao.");
        }

        // Kiểm tra xung đột: Phòng đã được bàn giao cho đơn khác
        if (request.BorrowRequestRooms.Any())
        {
            var roomIds = request.BorrowRequestRooms.Select(rr => rr.RoomId).ToList();
            var conflictingRoomCheckouts = await _context.Checkouts
                .Include(c => c.BorrowRequest)
                    .ThenInclude(br => br.BorrowRequestRooms)
                        .ThenInclude(brr => brr.Room)
                .Where(c => c.BorrowRequest.Status == RequestStatus.CheckedOut
                    && c.BorrowRequest.BorrowRequestRooms.Any(rr => roomIds.Contains(rr.RoomId))
                    && c.BorrowRequest.BorrowStartAt < request.BorrowEndAt
                    && c.BorrowRequest.BorrowEndAt > request.BorrowStartAt)
                .ToListAsync();

            if (conflictingRoomCheckouts.Any())
            {
                var conflict = conflictingRoomCheckouts.First();
                var roomName = conflict.BorrowRequest.BorrowRequestRooms.FirstOrDefault(rr => roomIds.Contains(rr.RoomId))?.Room?.RoomCode ?? "Phòng";
                throw new Exception($"⚠️ XUNG ĐỘT PHÒNG: {roomName} đang được mượn bởi đơn {conflict.BorrowRequest.RequestCode} từ {conflict.BorrowRequest.BorrowStartAt:HH:mm} đến {conflict.BorrowRequest.BorrowEndAt:HH:mm}. Không thể bàn giao.");
            }
        }

        var checkout = new Checkout
        {
            CheckoutCode = $"BG-{DateTime.Now:yyyyMMddHHmmss}",
            CheckoutAt = DateTime.Now,
            CheckoutNote = note,
            RequestId = request.RequestId,
            CheckedOutBy = checkedOutByUserId,
            CheckedOutTo = request.RequesterId
        };

        foreach (var reqAsset in request.BorrowRequestAssets)
        {
            var asset = reqAsset.Asset;
            if (asset == null) throw new Exception($"Không tìm thấy CSVC ID={reqAsset.AssetId}.");
            if (reqAsset.QuantityApproved <= 0)
                throw new Exception($"CSVC {asset.AssetName} chưa có số lượng được duyệt để bàn giao.");
            // Lưu ý: Tồn kho đã được trừ ở bước Duyệt (ApproveRequestAsync)
            // Ở đây chỉ tạo CheckoutItem để ghi nhận bàn giao thực tế

            var condition = assetConditions.ContainsKey(asset.AssetId) ? assetConditions[asset.AssetId] : "Tốt";

            checkout.CheckoutItems.Add(new CheckoutItem
            {
                ConditionBefore = condition,
                Quantity = reqAsset.QuantityApproved,
                IsReturned = false,
                RequestAssetId = reqAsset.RequestAssetId,
                AssetId = asset.AssetId
            });

            reqAsset.QuantityCheckedOut = reqAsset.QuantityApproved;
        }

        request.Status = RequestStatus.CheckedOut;
        _context.Checkouts.Add(checkout);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(checkedOutByUserId, "Bàn giao", "Checkout", checkout.CheckoutId, $"Quản lý bàn giao CSVC cho đơn {request.RequestCode}");

        return checkout;
    }

    /// <summary>
    /// Tạo nhanh đơn mượn vãng lai tại quầy (UC-09 Luồng 3b).
    /// Gộp: Tạo đơn → Duyệt → Bàn giao thành 1 bước duy nhất.
    /// </summary>
    public async Task<Checkout> CreateInstantCheckoutAsync(
        int studentUserId, int staffUserId, string note,
        List<BorrowRequestAssetDto> assets, int? roomId)
    {
        _context.ChangeTracker.Clear();

        var student = await _context.Users.FindAsync(studentUserId);
        if (student == null) throw new Exception("Không tìm thấy sinh viên.");
        if (student.Status == UserStatus.Locked)
            throw new Exception("Tài khoản sinh viên đang bị khóa, không thể mượn.");

        // Tạo đơn mượn trực tiếp
        var count = await _context.BorrowRequests.CountAsync() + 1;
        var request = new BorrowRequest
        {
            RequestCode = $"VL-{count:D3}",  // VL = Vãng Lai
            RequestType = RequestType.InClass,
            RequesterId = studentUserId,
            Title = "Mượn trực tiếp tại quầy",
            Purpose = note,
            BorrowStartAt = DateTime.Now,
            BorrowEndAt = DateTime.Now.AddHours(3),
            ExpectedReturnAt = DateTime.Now.AddHours(3),
            Status = RequestStatus.CheckedOut,  // Bỏ qua Pending/Approved
            ApprovedBy = staffUserId,
            ApprovedAt = DateTime.Now,
            PriorityLevel = PriorityLevel.Normal,
            CreatedAt = DateTime.Now
        };

        foreach (var item in assets)
        {
            if (item.AssetId <= 0)
                throw new Exception("Danh sách CSVC có mã không hợp lệ.");
            if (item.QuantityRequested <= 0)
                throw new Exception("Số lượng mượn phải lớn hơn 0.");

            var asset = await _context.Assets.FindAsync(item.AssetId);
            if (asset == null) throw new Exception($"Không tìm thấy CSVC ID={item.AssetId}.");
            if (asset.AvailabilityStatus != AvailabilityStatus.Available || asset.ConditionStatus == ConditionStatus.Damaged)
                throw new Exception($"CSVC {asset.AssetName} hiện không khả dụng để mượn.");
            if (asset.AvailableQuantity < item.QuantityRequested)
                throw new Exception($"Không đủ tồn kho: {asset.AssetName} (yêu cầu: {item.QuantityRequested}, khả dụng: {asset.AvailableQuantity})");

            // Trừ tồn kho
            asset.AvailableQuantity -= item.QuantityRequested;

            request.BorrowRequestAssets.Add(new BorrowRequestAsset
            {
                AssetId = item.AssetId,
                QuantityRequested = item.QuantityRequested,
                QuantityApproved = item.QuantityRequested,
                QuantityCheckedOut = item.QuantityRequested,
                QuantityReturned = 0,
                ItemNote = item.ItemNote
            });
        }

        if (roomId.HasValue)
        {
            request.BorrowRequestRooms.Add(new BorrowRequestRoom { RoomId = roomId.Value });
        }

        _context.BorrowRequests.Add(request);
        await _context.SaveChangesAsync();

        // Tạo phiếu bàn giao ngay
        var checkout = new Checkout
        {
            CheckoutCode = $"BG-VL-{DateTime.Now:yyyyMMddHHmmss}",
            CheckoutAt = DateTime.Now,
            CheckoutNote = $"Mượn vãng lai tại quầy: {note}",
            RequestId = request.RequestId,
            CheckedOutBy = staffUserId,
            CheckedOutTo = studentUserId
        };

        foreach (var reqAsset in request.BorrowRequestAssets)
        {
            checkout.CheckoutItems.Add(new CheckoutItem
            {
                ConditionBefore = "Tốt",
                Quantity = reqAsset.QuantityApproved,
                IsReturned = false,
                RequestAssetId = reqAsset.RequestAssetId,
                AssetId = reqAsset.AssetId
            });
        }

        _context.Checkouts.Add(checkout);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(staffUserId, "Bàn giao vãng lai", "Checkout", checkout.CheckoutId,
            $"QL tạo nhanh đơn vãng lai {request.RequestCode} cho SV {student.FullName}");

        return checkout;
    }

    public async Task<IEnumerable<Checkout>> GetAllCheckoutsAsync()
    {
        return await _context.Checkouts
            .Include(c => c.BorrowRequest)
            .Include(c => c.CheckedOutByUser)
            .Include(c => c.CheckedOutToUser)
            .ToListAsync();
    }

    public async Task<Checkout?> GetCheckoutByIdAsync(int checkoutId)
    {
        return await _context.Checkouts
            .Include(c => c.CheckoutItems)
            .ThenInclude(ci => ci.Asset)
            .FirstOrDefaultAsync(c => c.CheckoutId == checkoutId);
    }
}

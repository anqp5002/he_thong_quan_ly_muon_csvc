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
            if (asset.AvailableQuantity < reqAsset.QuantityApproved)
            {
                throw new Exception($"Không đủ số lượng khả dụng cho CSVC: {asset.AssetName}");
            }

            // Trừ số lượng tồn kho
            asset.AvailableQuantity -= reqAsset.QuantityApproved;

            var condition = assetConditions.ContainsKey(asset.AssetId) ? assetConditions[asset.AssetId] : "Tốt";

            checkout.CheckoutItems.Add(new CheckoutItem
            {
                ConditionBefore = condition,
                Quantity = reqAsset.QuantityApproved,
                IsReturned = false,
                RequestAssetId = reqAsset.RequestAssetId,
                AssetId = asset.AssetId
            });
        }

        request.Status = RequestStatus.CheckedOut;
        _context.Checkouts.Add(checkout);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(checkedOutByUserId, "Bàn giao", "Checkout", checkout.CheckoutId, $"Quản lý bàn giao CSVC cho đơn {request.RequestCode}");

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

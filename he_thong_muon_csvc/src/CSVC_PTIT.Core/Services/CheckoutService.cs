using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CSVC_PTIT.Core.Services;

public class CheckoutService : ICheckoutService
{
    private readonly CsvcDbContext _context;

    public CheckoutService(CsvcDbContext context)
    {
        _context = context;
    }

    public async Task<Checkout> CreateCheckoutAsync(int requestId, int checkedOutByUserId, string note, Dictionary<int, string> assetConditions)
    {
        var request = await _context.BorrowRequests
            .Include(r => r.BorrowRequestAssets)
            .ThenInclude(ra => ra.Asset)
            .FirstOrDefaultAsync(r => r.RequestId == requestId);

        if (request == null) throw new Exception("Không tìm thấy đơn mượn.");
        if (request.Status != RequestStatus.Approved) throw new Exception("Đơn mượn chưa được duyệt hoặc đã xử lý.");

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

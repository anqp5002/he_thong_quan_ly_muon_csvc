using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CSVC_PTIT.Core.Services;

public class ReturnService : IReturnService
{
    private readonly CsvcDbContext _context;

    public ReturnService(CsvcDbContext context)
    {
        _context = context;
    }

    public async Task<Return> CreateReturnAsync(int checkoutId, int receivedByUserId, string note, List<ReturnItemDto> returnItems)
    {
        if (returnItems == null || returnItems.Count == 0)
            throw new Exception("Vui lòng nhập ít nhất một CSVC cần nhận trả.");

        var checkout = await _context.Checkouts
            .Include(c => c.BorrowRequest)
                .ThenInclude(br => br.Requester)
            .Include(c => c.CheckoutItems)
                .ThenInclude(ci => ci.Asset)
            .Include(c => c.CheckoutItems)
                .ThenInclude(ci => ci.BorrowRequestAsset)
            .FirstOrDefaultAsync(c => c.CheckoutId == checkoutId);

        if (checkout == null) throw new Exception("Không tìm thấy phiếu bàn giao.");

        var request = checkout.BorrowRequest;
        var returnedAt = DateTime.Now;
        var lateMinutes = request.ExpectedReturnAt < returnedAt
            ? (int)(returnedAt - request.ExpectedReturnAt).TotalMinutes
            : 0;

        var returnObj = new Return
        {
            ReturnedAt = returnedAt,
            ReturnNote = note,
            LateMinutes = lateMinutes,
            CheckoutId = checkout.CheckoutId,
            ReceivedBy = receivedByUserId,
            ReturnedBy = checkout.CheckedOutTo
        };

        var processedItemCount = 0;

        foreach (var dto in returnItems)
        {
            var checkoutItem = checkout.CheckoutItems.FirstOrDefault(ci => ci.CheckoutItemId == dto.CheckoutItemId);
            if (checkoutItem == null) throw new Exception($"Không tìm thấy item bàn giao {dto.CheckoutItemId}.");
            if (checkoutItem.IsReturned)
                throw new Exception($"CSVC {checkoutItem.Asset.AssetName} đã được trả trước đó.");
            if (dto.AssetId != checkoutItem.AssetId)
                throw new Exception($"CSVC trả về không khớp với phiếu bàn giao {dto.CheckoutItemId}.");
            if (dto.QuantityReturned < 0)
                throw new Exception("Số lượng trả không được âm.");
            if (dto.QuantityReturned == 0 && !dto.IsDamaged && !dto.IsLost)
                continue;

            var remainingQuantity = checkoutItem.Quantity - checkoutItem.BorrowRequestAsset.QuantityReturned;
            if (dto.QuantityReturned > remainingQuantity)
                throw new Exception($"Số lượng trả của {checkoutItem.Asset.AssetName} vượt quá số lượng còn phải trả.");
            if ((dto.IsDamaged || dto.IsLost) && dto.QuantityReturned == 0)
                throw new Exception($"Số lượng hỏng/mất của {checkoutItem.Asset.AssetName} phải lớn hơn 0.");
            if ((dto.IsDamaged || dto.IsLost) && string.IsNullOrWhiteSpace(dto.DamageNote))
                throw new Exception($"Vui lòng nhập ghi chú sự cố cho {checkoutItem.Asset.AssetName}.");

            var returnItem = new ReturnItem
            {
                ConditionAfter = dto.ConditionAfter,
                QuantityReturned = dto.QuantityReturned,
                IsDamaged = dto.IsDamaged,
                IsLost = dto.IsLost,
                DamageNote = dto.DamageNote,
                CheckoutItemId = dto.CheckoutItemId,
                AssetId = dto.AssetId
            };

            returnObj.ReturnItems.Add(returnItem);
            processedItemCount++;

            var asset = checkoutItem.Asset;
            if (!dto.IsDamaged && !dto.IsLost)
            {
                asset.AvailableQuantity += dto.QuantityReturned;
            }
            else
            {
                asset.ConditionStatus = ConditionStatus.Damaged;
                if (dto.IsLost)
                {
                    asset.TotalQuantity = Math.Max(0, asset.TotalQuantity - dto.QuantityReturned);
                }
            }

            checkoutItem.BorrowRequestAsset.QuantityReturned += dto.QuantityReturned;
            checkoutItem.IsReturned = checkoutItem.BorrowRequestAsset.QuantityReturned >= checkoutItem.Quantity;
        }

        if (processedItemCount == 0)
            throw new Exception("Vui lòng nhập số lượng trả lớn hơn 0 cho ít nhất một CSVC.");

        var allItemsReturned = checkout.CheckoutItems.All(ci => ci.IsReturned);
        var hasDamage = returnItems.Any(r => r.IsDamaged || r.IsLost);

        if (allItemsReturned && !hasDamage)
        {
            request.Status = RequestStatus.Returned;
            request.ActualReturnAt = returnObj.ReturnedAt;
        }

        if (hasDamage && request.Requester != null)
        {
            request.Requester.Status = UserStatus.Locked;
        }

        _context.Returns.Add(returnObj);
        await _context.SaveChangesAsync();

        return returnObj;
    }

    public async Task<IEnumerable<Return>> GetAllReturnsAsync()
    {
        return await _context.Returns
            .Include(r => r.Checkout)
            .Include(r => r.ReceivedByUser)
            .Include(r => r.ReturnedByUser)
            .ToListAsync();
    }

    public async Task<Return?> GetReturnByIdAsync(int returnId)
    {
        return await _context.Returns
            .Include(r => r.ReturnItems)
                .ThenInclude(ri => ri.Asset)
            .FirstOrDefaultAsync(r => r.ReturnId == returnId);
    }
}

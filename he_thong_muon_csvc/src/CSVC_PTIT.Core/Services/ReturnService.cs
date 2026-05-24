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
        var checkout = await _context.Checkouts
            .Include(c => c.BorrowRequest)
            .Include(c => c.CheckoutItems)
            .ThenInclude(ci => ci.Asset)
            .FirstOrDefaultAsync(c => c.CheckoutId == checkoutId);

        if (checkout == null) throw new Exception("Không tìm thấy phiếu bàn giao.");

        var request = checkout.BorrowRequest;

        var returnObj = new Return
        {
            ReturnedAt = DateTime.Now,
            ReturnNote = note,
            CheckoutId = checkout.CheckoutId,
            ReceivedBy = receivedByUserId,
            ReturnedBy = checkout.CheckedOutTo
        };

        bool allItemsReturned = true;

        foreach (var dto in returnItems)
        {
            var checkoutItem = checkout.CheckoutItems.FirstOrDefault(ci => ci.CheckoutItemId == dto.CheckoutItemId);
            if (checkoutItem == null) throw new Exception($"Không tìm thấy item bàn giao {dto.CheckoutItemId}");

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

            var asset = checkoutItem.Asset;
            
            // Cộng lại số lượng khả dụng
            asset.AvailableQuantity += dto.QuantityReturned;

            if (dto.IsDamaged || dto.IsLost)
            {
                asset.ConditionStatus = dto.IsLost ? ConditionStatus.Damaged : ConditionStatus.Fair;
            }

            checkoutItem.IsReturned = true;
        }

        if (checkout.CheckoutItems.Any(ci => !ci.IsReturned))
        {
            allItemsReturned = false;
        }

        if (allItemsReturned)
        {
            request.Status = RequestStatus.Returned;
            request.ActualReturnAt = returnObj.ReturnedAt;
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

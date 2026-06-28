using CSVC_PTIT.Core.DTOs;
using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

public interface ICheckoutService
{
    Task<Checkout> CreateCheckoutAsync(int requestId, int checkedOutByUserId, string note, Dictionary<int, string> assetConditions);

    /// <summary>
    /// Tạo nhanh đơn mượn vãng lai tại quầy (UC-09 Luồng 3b).
    /// Gộp: Tạo đơn → Duyệt → Bàn giao thành 1 bước.
    /// </summary>
    Task<Checkout> CreateInstantCheckoutAsync(int studentUserId, int staffUserId, string note, List<BorrowRequestAssetDto> assets, int? roomId);

    Task<IEnumerable<Checkout>> GetAllCheckoutsAsync();
    Task<Checkout?> GetCheckoutByIdAsync(int checkoutId);
}

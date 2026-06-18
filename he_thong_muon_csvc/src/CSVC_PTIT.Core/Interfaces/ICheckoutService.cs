using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

public interface ICheckoutService
{
    Task<Checkout> CreateCheckoutAsync(int requestId, int checkedOutByUserId, string note, Dictionary<int, string> assetConditions);
    Task<IEnumerable<Checkout>> GetAllCheckoutsAsync();
    Task<Checkout?> GetCheckoutByIdAsync(int checkoutId);
}

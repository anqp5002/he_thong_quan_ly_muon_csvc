using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

public class ReturnItemDto
{
    public int CheckoutItemId { get; set; }
    public int AssetId { get; set; }
    public int QuantityReturned { get; set; }
    public string ConditionAfter { get; set; } = "Tốt";
    public bool IsDamaged { get; set; }
    public bool IsLost { get; set; }
    public string? DamageNote { get; set; }
}

public interface IReturnService
{
    Task<Return> CreateReturnAsync(int checkoutId, int receivedByUserId, string note, List<ReturnItemDto> returnItems);
    Task<IEnumerable<Return>> GetAllReturnsAsync();
    Task<Return?> GetReturnByIdAsync(int returnId);
}

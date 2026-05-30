using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

/// <summary>
/// Hợp đồng cho dịch vụ quản lý CSVC (UC-AD02).
/// Sprint 1 — Task A.6
/// </summary>
public interface IAssetService
{
    Task<List<Asset>> GetAllAsync();
    Task<Asset?> GetByIdAsync(int assetId);
    Task<Asset> CreateAsync(Asset asset);
    Task UpdateAsync(Asset asset);
    Task UpdateStatusAsync(int assetId, string status);
    Task DeleteAsync(int assetId);
    Task<List<AssetCategory>> GetCategoriesAsync();
    Task<AssetCategory> CreateCategoryAsync(AssetCategory category);
    Task UpdateCategoryAsync(AssetCategory category);
    Task<List<Room>> GetRoomsAsync();
}

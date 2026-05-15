using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Services;

/// <summary>
/// Service quản lý CSVC — stub.
/// Sprint 1 — Task A.6 sẽ code logic thật.
/// </summary>
public class AssetService : IAssetService
{
    private readonly CsvcDbContext _context;

    public AssetService(CsvcDbContext context)
    {
        _context = context;
    }

    public Task<List<Asset>> GetAllAsync()
        => throw new NotImplementedException("Sprint 1 — Task A.6");

    public Task<Asset?> GetByIdAsync(int assetId)
        => throw new NotImplementedException("Sprint 1 — Task A.6");

    public Task<Asset> CreateAsync(Asset asset)
        => throw new NotImplementedException("Sprint 1 — Task A.6");

    public Task UpdateAsync(Asset asset)
        => throw new NotImplementedException("Sprint 1 — Task A.6");

    public Task UpdateStatusAsync(int assetId, string status)
        => throw new NotImplementedException("Sprint 1 — Task A.6");

    public Task<List<AssetCategory>> GetCategoriesAsync()
        => throw new NotImplementedException("Sprint 1 — Task A.6");
}

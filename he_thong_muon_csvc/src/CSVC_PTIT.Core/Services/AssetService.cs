using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSVC_PTIT.Core.Services;

/// <summary>
/// Service quản lý CSVC — CRUD đầy đủ.
/// Sprint 1 — Task A.6
/// </summary>
public class AssetService : IAssetService
{
    private readonly CsvcDbContext _context;

    public AssetService(CsvcDbContext context)
    {
        _context = context;
    }

    public async Task<List<Asset>> GetAllAsync()
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.CurrentRoom)
            .ToListAsync();
    }

    public async Task<Asset?> GetByIdAsync(int assetId)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.CurrentRoom)
            .FirstOrDefaultAsync(a => a.AssetId == assetId);
    }

    public async Task<Asset> CreateAsync(Asset asset)
    {
        if (await _context.Assets.AnyAsync(a => a.AssetCode == asset.AssetCode))
            throw new Exception("Mã CSVC đã tồn tại.");

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();
        return asset;
    }

    public async Task UpdateAsync(Asset asset)
    {
        var existing = await _context.Assets.FindAsync(asset.AssetId);
        if (existing == null)
            throw new Exception("Không tìm thấy CSVC.");

        if (existing.AssetCode != asset.AssetCode &&
            await _context.Assets.AnyAsync(a => a.AssetCode == asset.AssetCode))
            throw new Exception("Mã CSVC đã tồn tại.");

        existing.AssetCode = asset.AssetCode;
        existing.AssetName = asset.AssetName;
        existing.CategoryId = asset.CategoryId;
        existing.ManagementMode = asset.ManagementMode;
        existing.Unit = asset.Unit;
        existing.TotalQuantity = asset.TotalQuantity;
        existing.AvailableQuantity = asset.AvailableQuantity;
        existing.SerialNumber = asset.SerialNumber;
        existing.Brand = asset.Brand;
        existing.Model = asset.Model;
        existing.ConditionStatus = asset.ConditionStatus;
        existing.AvailabilityStatus = asset.AvailabilityStatus;
        existing.CurrentRoomId = asset.CurrentRoomId;
        existing.Description = asset.Description;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int assetId, string status)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null)
            throw new Exception("Không tìm thấy CSVC.");

        if (Enum.TryParse<ConditionStatus>(status, out var conditionStatus))
            asset.ConditionStatus = conditionStatus;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int assetId)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset != null)
        {
            asset.AvailabilityStatus = AvailabilityStatus.Unavailable;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<AssetCategory>> GetCategoriesAsync()
    {
        return await _context.AssetCategories.ToListAsync();
    }

    public async Task<AssetCategory> CreateCategoryAsync(AssetCategory category)
    {
        _context.AssetCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task UpdateCategoryAsync(AssetCategory category)
    {
        var existing = await _context.AssetCategories.FindAsync(category.CategoryId);
        if (existing == null)
            throw new Exception("Không tìm thấy loại CSVC.");

        existing.CategoryCode = category.CategoryCode;
        existing.CategoryName = category.CategoryName;
        existing.Description = category.Description;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Room>> GetRoomsAsync()
    {
        return await _context.Rooms.ToListAsync();
    }
}

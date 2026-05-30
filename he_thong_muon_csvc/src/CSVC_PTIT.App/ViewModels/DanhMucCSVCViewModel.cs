using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CSVC_PTIT.App.ViewModels;

/// <summary>
/// ViewModel cho màn hình Danh mục CSVC (UC-AD02, AD_BM02).
/// Sprint 1 — Task A.7
/// </summary>
public partial class DanhMucCSVCViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;
    private List<Asset> _allAssets = new();

    [ObservableProperty]
    private ObservableCollection<Asset> _assets = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private List<AssetCategory> _categories = new();

    [ObservableProperty]
    private AssetCategory? _selectedCategory;

    [ObservableProperty]
    private List<Room> _rooms = new();

    public DanhMucCSVCViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnSelectedCategoryChanged(AssetCategory? value) => ApplyFilter();

    private void ApplyFilter()
    {
        var filtered = _allAssets.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(a =>
                (a.AssetCode?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (a.AssetName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (SelectedCategory != null)
        {
            filtered = filtered.Where(a => a.CategoryId == SelectedCategory.CategoryId);
        }

        Assets = new ObservableCollection<Asset>(filtered);
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            using var scope = _scopeFactory.CreateScope();
            var assetService = scope.ServiceProvider.GetRequiredService<IAssetService>();

            _allAssets = await assetService.GetAllAsync();
            Categories = await assetService.GetCategoriesAsync();
            Rooms = await assetService.GetRoomsAsync();

            ApplyFilter();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task CreateAssetAsync(Asset newAsset)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IAssetService>();
            await service.CreateAsync(newAsset);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    public async Task UpdateAssetAsync(Asset asset)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IAssetService>();
            await service.UpdateAsync(asset);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    public async Task DeleteAssetAsync(Asset asset)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IAssetService>();
            await service.DeleteAsync(asset.AssetId);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}

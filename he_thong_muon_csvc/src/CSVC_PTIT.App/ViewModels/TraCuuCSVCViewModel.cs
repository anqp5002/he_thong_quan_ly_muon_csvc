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
/// ViewModel cho màn hình Tra cứu CSVC khả dụng (UC-SV01, SV_BM01).
/// Sprint 1 — Task B.1
/// </summary>
public partial class TraCuuCSVCViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;
    private List<Asset> _allAssets = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private List<AssetCategory> _categories = new();

    [ObservableProperty]
    private AssetCategory? _selectedCategory;

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private ObservableCollection<Asset> _assets = new();

    [ObservableProperty]
    private Asset? _selectedAsset;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _resultSummary = string.Empty;

    public TraCuuCSVCViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
       
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnSelectedCategoryChanged(AssetCategory? value) => ApplyFilter();
    partial void OnSelectedDateChanged(DateTime value) => ApplyFilter();

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

        var result = filtered.ToList();
        Assets = new ObservableCollection<Asset>(result);
        ResultSummary = result.Count > 0
            ? $"Tìm thấy {result.Count} CSVC khả dụng"
            : "Không có CSVC khả dụng phù hợp";
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            using var scope = _scopeFactory.CreateScope();
            var assetService = scope.ServiceProvider.GetRequiredService<IAssetService>();

            var all = await assetService.GetAllAsync();
            _allAssets = all.Where(a => a.AvailableQuantity > 0).ToList();

            // Load danh mục loại CSVC cho combobox lọc
            Categories = await assetService.GetCategoriesAsync();

            ApplyFilter();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải dữ liệu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void ClearFilter()
    {
        SearchText = string.Empty;
        SelectedCategory = null;
        SelectedDate = DateTime.Today;
    }

    [RelayCommand]
    public async Task RefreshAsync() => await LoadDataAsync();
}
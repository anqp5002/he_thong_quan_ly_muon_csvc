using CSVC_PTIT.App.ViewModels;
using CSVC_PTIT.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace CSVC_PTIT.App.Views;

/// <summary>
/// Màn hình Danh mục CSVC (UC-AD02, AD_BM02).
/// Sprint 1 — Task A.7
/// </summary>
public partial class DanhMucCSVCView : UserControl
{
    private readonly DanhMucCSVCViewModel _viewModel;

    public DanhMucCSVCView()
    {
        InitializeComponent();
        _viewModel = App.ServiceProvider.GetRequiredService<DanhMucCSVCViewModel>();
        DataContext = _viewModel;
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadDataAsync();
    }

    private async void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AssetDialogView(_viewModel.Categories, _viewModel.Rooms)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true && dialog.ResultAsset != null)
        {
            await _viewModel.CreateAssetAsync(dialog.ResultAsset);
        }
    }

    private async void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Asset asset)
        {
            var dialog = new AssetDialogView(_viewModel.Categories, _viewModel.Rooms, asset)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true && dialog.ResultAsset != null)
            {
                await _viewModel.UpdateAssetAsync(dialog.ResultAsset);
            }
        }
    }

    private async void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Asset asset)
        {
            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa CSVC \"{asset.AssetName}\" ({asset.AssetCode})?\n(Sẽ đánh dấu không khả dụng)",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                await _viewModel.DeleteAssetAsync(asset);
        }
    }
}

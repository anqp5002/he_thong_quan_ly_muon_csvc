using CSVC_PTIT.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace CSVC_PTIT.App.Views;

/// <summary>
/// Màn hình Nhật ký hoạt động (UC-AD04, AD_BM04).
/// Sprint 1 — Task A.9
/// </summary>
public partial class NhatKyView : UserControl
{
    private readonly NhatKyViewModel _viewModel;

    public NhatKyView()
    {
        InitializeComponent();
        _viewModel = App.ServiceProvider.GetRequiredService<NhatKyViewModel>();
        DataContext = _viewModel;
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadDataAsync();
    }
}

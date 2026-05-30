using CSVC_PTIT.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace CSVC_PTIT.App.Views;

/// <summary>
/// Màn hình Cấu hình hệ thống (UC-AD03, AD_BM03).
/// Sprint 1 — Task A.8
/// </summary>
public partial class CauHinhHeThongView : UserControl
{
    private readonly CauHinhHeThongViewModel _viewModel;

    public CauHinhHeThongView()
    {
        InitializeComponent();
        _viewModel = App.ServiceProvider.GetRequiredService<CauHinhHeThongViewModel>();
        DataContext = _viewModel;
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadDataAsync();
    }
}

using CSVC_PTIT.App.ViewModels;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace CSVC_PTIT.App.Views;

/// <summary>
/// Màn hình Quản lý tài khoản (UC-AD01).
/// Sprint 1 — Task A.5
/// </summary>
public partial class QuanLyTaiKhoanView : UserControl
{
    private readonly QuanLyTaiKhoanViewModel _viewModel;

    public QuanLyTaiKhoanView()
    {
        InitializeComponent();
        _viewModel = App.ServiceProvider.GetRequiredService<QuanLyTaiKhoanViewModel>();
        DataContext = _viewModel;
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadDataAsync();
    }

    private async void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new UserDialogView(_viewModel.Roles, _viewModel.Departments)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true && dialog.ResultUser != null)
        {
            await _viewModel.CreateUserAsync(dialog.ResultUser);
        }
    }

    private async void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is User user)
        {
            var dialog = new UserDialogView(_viewModel.Roles, _viewModel.Departments, user)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true && dialog.ResultUser != null)
            {
                await _viewModel.UpdateUserAsync(dialog.ResultUser);
            }
        }
    }

    private async void BtnToggleLock_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is User user)
        {
            var action = user.Status == UserStatus.Locked ? "mở khóa" : "khóa";
            var result = MessageBox.Show(
                $"Bạn có chắc muốn {action} tài khoản \"{user.FullName}\"?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                await _viewModel.ToggleLockAsync(user);
        }
    }

    private async void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is User user)
        {
            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa tài khoản \"{user.FullName}\"?\n(Xóa mềm — có thể khôi phục)",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                await _viewModel.DeleteUserAsync(user);
        }
    }
}

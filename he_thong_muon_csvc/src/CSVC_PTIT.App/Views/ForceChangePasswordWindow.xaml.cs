using System;
using System.Windows;
using CSVC_PTIT.Core.Interfaces;

namespace CSVC_PTIT.App.Views;

public partial class ForceChangePasswordWindow : Window
{
    private readonly IAuthService _authService;
    private readonly string _email;

    public ForceChangePasswordWindow(string email, IAuthService authService)
    {
        InitializeComponent();
        _email = email;
        _authService = authService;
    }

    private async void BtnChange_Click(object sender, RoutedEventArgs e)
    {
        TxtError.Visibility = Visibility.Collapsed;

        if (string.IsNullOrWhiteSpace(TxtNewPassword.Password) || string.IsNullOrWhiteSpace(TxtConfirmPassword.Password))
        {
            TxtError.Text = "Vui lòng nhập đầy đủ mật khẩu mới.";
            TxtError.Visibility = Visibility.Visible;
            return;
        }

        if (TxtNewPassword.Password == "123456")
        {
            TxtError.Text = "Mật khẩu mới không được giống mật khẩu mặc định (123456).";
            TxtError.Visibility = Visibility.Visible;
            return;
        }

        if (TxtNewPassword.Password != TxtConfirmPassword.Password)
        {
            TxtError.Text = "Mật khẩu xác nhận không khớp.";
            TxtError.Visibility = Visibility.Visible;
            return;
        }

        try
        {
            ProgressBusy.Visibility = Visibility.Visible;
            BtnChange.IsEnabled = false;

            // CurrentUser was populated by LoginAsync before throwing MUST_CHANGE_PASSWORD
            if (_authService.CurrentUser != null)
            {
                await _authService.ChangePasswordAsync(_authService.CurrentUser.UserId, "123456", TxtNewPassword.Password);
                
                MessageBox.Show("Đổi mật khẩu thành công! Bạn có thể bắt đầu sử dụng hệ thống.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                TxtError.Text = "Lỗi phiên đăng nhập. Vui lòng đăng nhập lại.";
                TxtError.Visibility = Visibility.Visible;
            }
        }
        catch (Exception ex)
        {
            TxtError.Text = ex.Message;
            TxtError.Visibility = Visibility.Visible;
        }
        finally
        {
            ProgressBusy.Visibility = Visibility.Collapsed;
            BtnChange.IsEnabled = true;
        }
    }
}

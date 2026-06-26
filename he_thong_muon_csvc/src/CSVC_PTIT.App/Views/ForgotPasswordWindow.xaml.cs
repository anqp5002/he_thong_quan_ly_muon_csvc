using System;
using System.Windows;
using CSVC_PTIT.Core.Interfaces;

namespace CSVC_PTIT.App.Views;

public partial class ForgotPasswordWindow : Window
{
    private readonly IAuthService _authService;

    public ForgotPasswordWindow(IAuthService authService, string defaultEmail = "")
    {
        InitializeComponent();
        _authService = authService;
        TxtEmail.Text = defaultEmail;
    }

    private void ShowMessage(string message, bool isError = false)
    {
        TxtMessage.Visibility = Visibility.Collapsed;
        TxtError.Visibility = Visibility.Collapsed;
        
        if (isError)
        {
            TxtError.Text = message;
            TxtError.Visibility = Visibility.Visible;
        }
        else
        {
            TxtMessage.Text = message;
            TxtMessage.Visibility = Visibility.Visible;
        }
    }

    private async void BtnSendOtp_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtEmail.Text))
        {
            ShowMessage("Vui lòng nhập email.", true);
            return;
        }

        try
        {
            ProgressBusy.Visibility = Visibility.Visible;
            BtnSendOtp.IsEnabled = false;

            await _authService.SendForgotPasswordOtpAsync(TxtEmail.Text);

            ShowMessage("Mã OTP đã được gửi đến email của bạn.");
            PanelReset.Visibility = Visibility.Visible;
            TxtEmail.IsEnabled = false;
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
        finally
        {
            ProgressBusy.Visibility = Visibility.Collapsed;
            BtnSendOtp.IsEnabled = true;
        }
    }

    private async void BtnResetPassword_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtOtp.Text) || string.IsNullOrWhiteSpace(TxtNewPassword.Password))
        {
            ShowMessage("Vui lòng nhập đầy đủ mã OTP và mật khẩu mới.", true);
            return;
        }

        try
        {
            ProgressBusy.Visibility = Visibility.Visible;
            BtnResetPassword.IsEnabled = false;

            await _authService.ResetPasswordWithOtpAsync(TxtEmail.Text, TxtOtp.Text, TxtNewPassword.Password);

            MessageBox.Show("Khôi phục mật khẩu thành công! Vui lòng đăng nhập lại.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
        finally
        {
            ProgressBusy.Visibility = Visibility.Collapsed;
            BtnResetPassword.IsEnabled = true;
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace CSVC_PTIT.App.ViewModels;

public partial class ChangePasswordViewModel : ViewModelBase
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _oldPassword = string.Empty;

    [ObservableProperty]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public Action? OnSuccess { get; set; }

    public ChangePasswordViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
            return;
        }

        if (NewPassword.Length < 6)
        {
            ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Mật khẩu xác nhận không khớp.";
            return;
        }

        if (_authService.CurrentUser == null)
        {
            ErrorMessage = "Bạn chưa đăng nhập.";
            return;
        }

        try
        {
            IsBusy = true;
            await _authService.ChangePasswordAsync(_authService.CurrentUser.UserId, OldPassword, NewPassword);
            OnSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

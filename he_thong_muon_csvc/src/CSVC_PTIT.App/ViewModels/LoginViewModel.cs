using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CSVC_PTIT.App.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private DispatcherTimer? _lockoutTimer;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLockedOut;

    [ObservableProperty]
    private string _lockoutMessage = string.Empty;

    public Action? OnLoginSuccess { get; set; }

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Vui lòng nhập email và mật khẩu.";
            return;
        }

        try
        {
            ErrorMessage = string.Empty;
            IsBusy = true;
            
            var user = await _authService.LoginAsync(Email, Password);
            if (user != null)
            {
                OnLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = "Email hoặc mật khẩu không đúng.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            if (_authService.LockoutEndTime.HasValue)
            {
                StartLockoutTimer();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void StartLockoutTimer()
    {
        if (_authService.LockoutEndTime == null) return;
        
        IsLockedOut = true;
        _lockoutTimer?.Stop();
        
        _lockoutTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _lockoutTimer.Tick += (s, e) =>
        {
            var remaining = _authService.LockoutEndTime.Value - DateTime.Now;
            if (remaining.TotalSeconds <= 0)
            {
                IsLockedOut = false;
                LockoutMessage = string.Empty;
                _lockoutTimer.Stop();
            }
            else
            {
                LockoutMessage = $"Vui lòng thử lại sau {(int)remaining.TotalSeconds} giây";
            }
        };
        _lockoutTimer.Start();
    }
}

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.ViewModels;

public partial class ThongBaoViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private ObservableCollection<Notification> _notifications = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ThongBaoViewModel(IServiceScopeFactory scopeFactory, IAuthService authService)
    {
        _scopeFactory = scopeFactory;
        _authService = authService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var user = _authService.CurrentUser;
            if (user == null) return;

            using var scope = _scopeFactory.CreateScope();
            var notifService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var list = await notifService.GetUnreadNotificationsAsync(user.UserId);

            Notifications.Clear();
            foreach (var n in list)
                Notifications.Add(n);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải thông báo: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task MarkAsReadAsync(Notification notification)
    {
        if (notification == null) return;
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var notifService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            await notifService.MarkAsReadAsync(notification.Id);
            Notifications.Remove(notification);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task MarkAllAsReadAsync()
    {
        try
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            using var scope = _scopeFactory.CreateScope();
            var notifService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            await notifService.MarkAllAsReadAsync(user.UserId);
            Notifications.Clear();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi: {ex.Message}";
        }
    }
}

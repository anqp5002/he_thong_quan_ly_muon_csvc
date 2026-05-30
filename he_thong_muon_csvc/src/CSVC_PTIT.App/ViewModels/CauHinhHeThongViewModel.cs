using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CSVC_PTIT.App.ViewModels;

/// <summary>
/// ViewModel cho màn hình Cấu hình hệ thống (UC-AD03, AD_BM03).
/// Sprint 1 — Task A.8
/// </summary>
public partial class CauHinhHeThongViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;

    [ObservableProperty]
    private ObservableCollection<SystemConfig> _configs = new();

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public CauHinhHeThongViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CsvcDbContext>();
            var configs = await context.SystemConfigs.ToListAsync();
            Configs = new ObservableCollection<SystemConfig>(configs);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SaveAllAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = string.Empty;

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CsvcDbContext>();

            foreach (var config in Configs)
            {
                var existing = await context.SystemConfigs.FindAsync(config.ConfigId);
                if (existing != null)
                {
                    existing.ConfigValue = config.ConfigValue;
                    existing.IsActive = config.IsActive;
                    existing.UpdatedAt = DateTime.Now;
                }
            }

            await context.SaveChangesAsync();
            StatusMessage = "✅ Đã lưu thay đổi thành công!";
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

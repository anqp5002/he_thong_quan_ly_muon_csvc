using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CSVC_PTIT.App.ViewModels;

/// <summary>
/// ViewModel cho màn hình Nhật ký hoạt động (UC-AD04, AD_BM04).
/// Sprint 1 — Task A.9
/// </summary>
public partial class NhatKyViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;

    [ObservableProperty]
    private ObservableCollection<AuditLog> _logs = new();

    [ObservableProperty]
    private DateTime? _fromDate;

    [ObservableProperty]
    private DateTime? _toDate;

    [ObservableProperty]
    private string _filterAction = string.Empty;

    public NhatKyViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        // Mặc định lọc 7 ngày gần nhất
        FromDate = DateTime.Today.AddDays(-7);
        ToDate = DateTime.Today;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IAuditLogService>();

            var action = string.IsNullOrWhiteSpace(FilterAction) ? null : FilterAction;
            var logs = await service.GetLogsAsync(FromDate, ToDate, null, action);

            Logs = new ObservableCollection<AuditLog>(logs);
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
}

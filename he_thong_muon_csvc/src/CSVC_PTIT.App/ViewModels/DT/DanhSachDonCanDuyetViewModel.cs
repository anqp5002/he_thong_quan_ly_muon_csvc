using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.ViewModels.DT
{
    public partial class DanhSachDonCanDuyetViewModel : ViewModelBase
    {
        private readonly IServiceScopeFactory _scopeFactory;

        [ObservableProperty]
        private ObservableCollection<BorrowRequest> _pendingRequests = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public DanhSachDonCanDuyetViewModel(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                using var scope = _scopeFactory.CreateScope();
                var borrowService = scope.ServiceProvider.GetRequiredService<IBorrowService>();

                var requests = await borrowService.GetRequestsByStatusAsync(RequestStatus.Pending);
                
                PendingRequests.Clear();
                foreach (var req in requests)
                {
                    PendingRequests.Add(req);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải dữ liệu: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task ApproveRequestAsync(BorrowRequest request)
        {
            if (request == null) return;
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var borrowService = scope.ServiceProvider.GetRequiredService<IBorrowService>();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                
                // Assuming CurrentUser exists and has an ID. We use 1 as fallback admin ID if null for testing.
                int approverId = authService.CurrentUser?.UserId ?? 1;

                await borrowService.ApproveRequestAsync(request.RequestId, approverId);
                PendingRequests.Remove(request);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi duyệt đơn: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task RejectRequestAsync(BorrowRequest request)
        {
            if (request == null) return;
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var borrowService = scope.ServiceProvider.GetRequiredService<IBorrowService>();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                
                int approverId = authService.CurrentUser?.UserId ?? 1;

                await borrowService.RejectRequestAsync(request.RequestId, approverId, "Từ chối bởi Đoàn thể");
                PendingRequests.Remove(request);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi từ chối đơn: {ex.Message}";
            }
        }
    }
}

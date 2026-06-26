using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.ViewModels.QL
{
    public partial class DanhSachDonDaDuyetViewModel : ViewModelBase
    {
        private readonly IServiceScopeFactory _scopeFactory;

        [ObservableProperty]
        private ObservableCollection<BorrowRequest> _approvedRequests = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public DanhSachDonDaDuyetViewModel(IServiceScopeFactory scopeFactory)
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

                // Get both Approved and CheckedOut to see active requests
                var approved = await borrowService.GetRequestsByStatusAsync(RequestStatus.Approved);
                var checkedOut = await borrowService.GetRequestsByStatusAsync(RequestStatus.CheckedOut);
                
                ApprovedRequests.Clear();
                foreach (var req in approved)
                {
                    ApprovedRequests.Add(req);
                }
                foreach (var req in checkedOut)
                {
                    ApprovedRequests.Add(req);
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
        public async Task CheckAssetsAsync(BorrowRequest request)
        {
            if (request == null) return;
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CSVC_PTIT.Data.CsvcDbContext>();

                // Load the request with its assets to compare
                var fullRequest = await dbContext.BorrowRequests
                    .Include(r => r.BorrowRequestAssets)
                        .ThenInclude(ra => ra.Asset)
                    .FirstOrDefaultAsync(r => r.RequestId == request.RequestId);

                if (fullRequest == null) return;

                string message = $"Kết quả kiểm tra kho cho đơn {fullRequest.RequestCode}:\n\n";
                bool allAvailable = true;

                foreach (var ra in fullRequest.BorrowRequestAssets)
                {
                    int requested = ra.QuantityRequested;
                    int available = ra.Asset.AvailableQuantity;
                    string status = requested <= available ? "ĐỦ" : "THIẾU";
                    if (requested > available) allAvailable = false;

                    message += $"- {ra.Asset.AssetName}: Yêu cầu {requested}, Có sẵn {available} => {status}\n";
                }

                message += "\nKết luận: " + (allAvailable ? "CÓ THỂ BÀN GIAO." : "KHÔNG ĐỦ TÀI SẢN ĐỂ BÀN GIAO.");
                
                System.Windows.MessageBox.Show(message, "Kiểm tra Tồn Kho", System.Windows.MessageBoxButton.OK, allAvailable ? System.Windows.MessageBoxImage.Information : System.Windows.MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi kiểm tra kho: {ex.Message}";
            }
        }
    }
}

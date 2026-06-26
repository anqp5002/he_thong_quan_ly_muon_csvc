using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views.QL;

public partial class BanGiaoCSVCView : UserControl
{
    private readonly ICheckoutService _checkoutService;
    private readonly IReportService _reportService;
    private readonly CsvcDbContext _context;

    private BorrowRequest? _selectedRequest;
    private int _lastCheckoutId = 0;
    private List<BorrowRequest> _allRequests = new();

    public BanGiaoCSVCView()
    {
        InitializeComponent();
        
        _checkoutService = App.ServiceProvider.GetRequiredService<ICheckoutService>();
        _reportService = App.ServiceProvider.GetRequiredService<IReportService>();
        _context = App.ServiceProvider.GetRequiredService<CsvcDbContext>();

        LoadData();
    }

    private void LoadData()
    {
        // Tải danh sách đơn đã duyệt (C.1)
        _allRequests = _context.BorrowRequests
            .Include(r => r.Requester)
            .Include(r => r.BorrowRequestAssets)
            .ThenInclude(ra => ra.Asset)
            .Where(r => r.Status == RequestStatus.Approved)
            .ToList();
        
        GridDanhSachDon.ItemsSource = _allRequests;
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = TxtSearch.Text.Trim().ToLower();
        if (string.IsNullOrEmpty(text))
        {
            GridDanhSachDon.ItemsSource = _allRequests;
        }
        else
        {
            var filtered = _allRequests.Where(r => 
                (r.RequestCode != null && r.RequestCode.ToLower().Contains(text)) ||
                (r.Requester != null && r.Requester.StudentCode != null && r.Requester.StudentCode.ToLower().Contains(text)) ||
                (r.Requester != null && r.Requester.FullName != null && r.Requester.FullName.ToLower().Contains(text))
            ).ToList();
            GridDanhSachDon.ItemsSource = filtered;
        }
    }

    // Chuyển dữ liệu sang Tab 2 (Kiểm tra khả dụng)
    private void GridDanhSachDon_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridDanhSachDon.SelectedItem is BorrowRequest selected)
        {
            _selectedRequest = selected;
            // Hiển thị danh sách tài sản yêu cầu kèm tồn kho hiện tại (C.2)
            GridKiemTra.ItemsSource = selected.BorrowRequestAssets;
            // Chuyển sang Tab 2 tự động
            if (this.FindName("TabKiemTra") is TabItem tabKiemTra)
            {
                tabKiemTra.IsSelected = true;
            }
        }
    }

    // Nút xác nhận kiểm tra
    private void BtnKiemTraHopLe_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRequest == null)
        {
            MessageBox.Show("Vui lòng chọn đơn ở Tab 1 trước.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Kiểm tra logic tồn kho
        bool isValid = true;
        foreach (var reqAsset in _selectedRequest.BorrowRequestAssets)
        {
            if (reqAsset.QuantityApproved > reqAsset.Asset.AvailableQuantity)
            {
                isValid = false;
                MessageBox.Show($"Tài sản {reqAsset.Asset.AssetName} không đủ số lượng tồn kho!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Error);
                break;
            }
        }

        if (isValid)
        {
            // Chuyển sang Tab 3
            TxtNguoiNhan.Text = $"Người nhận: {_selectedRequest.Requester.FullName} - {_selectedRequest.Requester.Phone}";
            GridChiTietBanGiao.ItemsSource = _selectedRequest.BorrowRequestAssets;
            ((TabItem)this.FindName("TabPhieuBanGiao")).IsSelected = true;
        }
    }

    // Hoàn tất bàn giao (C.3 & C.4)
    private async void BtnHoanTatBanGiao_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRequest == null) return;

        try
        {
            var conditions = new Dictionary<int, string>();
            foreach (var reqAsset in _selectedRequest.BorrowRequestAssets)
            {
                // Lấy ghi chú tình trạng từ DataGrid (cột ItemNote)
                string condition = string.IsNullOrWhiteSpace(reqAsset.ItemNote) ? "Tốt" : reqAsset.ItemNote;
                conditions[reqAsset.Asset.AssetId] = condition;
            }

            var checkout = await _checkoutService.CreateCheckoutAsync(
                _selectedRequest.RequestId, 
                1 /* Thay bằng ID admin đang đăng nhập */, 
                TxtGhiChuBanGiao.Text, 
                conditions);
            
            _lastCheckoutId = checkout.CheckoutId;
            MessageBox.Show("Đã hoàn tất bàn giao và trừ tồn kho thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            
            BtnInPhieu.IsEnabled = true;
            BtnHoanTatBanGiao.IsEnabled = false;
            
            // Reload lại danh sách đơn chờ
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi bàn giao: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // In phiếu bàn giao (C.10)
    private async void BtnInPhieu_Click(object sender, RoutedEventArgs e)
    {
        if (_lastCheckoutId == 0) return;

        try
        {
            var pdfBytes = await _reportService.GenerateCheckoutPdfAsync(_lastCheckoutId);
            
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"PhieuBanGiao_BG_{_lastCheckoutId}.pdf");
            File.WriteAllBytes(path, pdfBytes);
            
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi in phiếu PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

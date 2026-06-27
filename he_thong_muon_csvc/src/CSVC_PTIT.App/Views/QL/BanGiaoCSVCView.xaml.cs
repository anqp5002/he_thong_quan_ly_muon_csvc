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

// ViewModel cho hiển thị danh sách checkout kèm trạng thái quá hạn
public class CheckoutDisplayItem
{
    public Checkout Checkout { get; set; } = null!;
    public BorrowRequest BorrowRequest => Checkout.BorrowRequest;
    public string CheckoutCode => Checkout.CheckoutCode;
    public User CheckedOutToUser => Checkout.CheckedOutToUser;

    public string OverdueStatus
    {
        get
        {
            if (BorrowRequest.ExpectedReturnAt < DateTime.Now)
            {
                var overtime = DateTime.Now - BorrowRequest.ExpectedReturnAt;
                if (overtime.TotalHours >= 1)
                    return $"⛔ QUÁ HẠN {overtime.Hours}h{overtime.Minutes:D2}p";
                else
                    return $"⚠️ Quá hạn {overtime.Minutes}p";
            }
            return "✅ Trong hạn";
        }
    }

    public bool IsOverdue1Hour =>
        BorrowRequest.ExpectedReturnAt < DateTime.Now &&
        (DateTime.Now - BorrowRequest.ExpectedReturnAt).TotalHours >= 1;

    public bool IsOverdueNormal =>
        BorrowRequest.ExpectedReturnAt < DateTime.Now &&
        (DateTime.Now - BorrowRequest.ExpectedReturnAt).TotalHours < 1;
}

// ViewModel cho chi tiết từng item trả
public class ReturnItemUI
{
    public int CheckoutItemId { get; set; }
    public Asset Asset { get; set; } = null!;
    public int Quantity { get; set; }
    public int QuantityReturned { get; set; }
    public string ConditionAfter { get; set; } = "Tốt";
    public string? DamageNote { get; set; }
}

public partial class BanGiaoCSVCView : UserControl
{
    private readonly ICheckoutService _checkoutService;
    private readonly IReturnService _returnService;
    private readonly IReportService _reportService;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;
    private readonly CsvcDbContext _context;

    private BorrowRequest? _selectedRequest;
    private Checkout? _selectedCheckout;
    private int _lastCheckoutId = 0;
    private int _lastReturnId = 0;
    private List<BorrowRequest> _allRequests = new();

    public BanGiaoCSVCView()
    {
        InitializeComponent();
        
        _checkoutService = App.ServiceProvider.GetRequiredService<ICheckoutService>();
        _returnService = App.ServiceProvider.GetRequiredService<IReturnService>();
        _reportService = App.ServiceProvider.GetRequiredService<IReportService>();
        _authService = App.ServiceProvider.GetRequiredService<IAuthService>();
        _notificationService = App.ServiceProvider.GetRequiredService<INotificationService>();
        _context = App.ServiceProvider.GetRequiredService<CsvcDbContext>();

        LoadData();
        LoadCheckedOutData();
    }

    // ========== TAB 1-3: BÀN GIAO ==========

    private void LoadData()
    {
        _context.ChangeTracker.Clear();
        _allRequests = _context.BorrowRequests
            .AsNoTracking()
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

    private void GridDanhSachDon_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridDanhSachDon.SelectedItem is BorrowRequest selected)
        {
            _selectedRequest = selected;
            GridKiemTra.ItemsSource = selected.BorrowRequestAssets;
            if (this.FindName("TabKiemTra") is TabItem tabKiemTra)
            {
                tabKiemTra.IsSelected = true;
            }
        }
    }

    private void BtnKiemTraHopLe_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRequest == null)
        {
            MessageBox.Show("Vui lòng chọn đơn ở Tab 1 trước.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

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
            TxtNguoiNhan.Text = $"Người nhận: {_selectedRequest.Requester.FullName} - {_selectedRequest.Requester.Phone}";
            GridChiTietBanGiao.ItemsSource = _selectedRequest.BorrowRequestAssets;
            ((TabItem)this.FindName("TabPhieuBanGiao")).IsSelected = true;
        }
    }

    private async void BtnHoanTatBanGiao_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRequest == null) return;

        try
        {
            var conditions = new Dictionary<int, string>();
            foreach (var reqAsset in _selectedRequest.BorrowRequestAssets)
            {
                string condition = string.IsNullOrWhiteSpace(reqAsset.ItemNote) ? "Tốt" : reqAsset.ItemNote;
                conditions[reqAsset.Asset.AssetId] = condition;
            }

            var currentUserId = _authService.CurrentUser?.UserId ?? 1;
            var checkout = await _checkoutService.CreateCheckoutAsync(
                _selectedRequest.RequestId, 
                currentUserId, 
                TxtGhiChuBanGiao.Text, 
                conditions);
            
            _lastCheckoutId = checkout.CheckoutId;
            MessageBox.Show("Đã hoàn tất bàn giao và trừ tồn kho thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            
            BtnInPhieu.IsEnabled = true;
            BtnHoanTatBanGiao.IsEnabled = false;
            
            LoadData();
            LoadCheckedOutData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi bàn giao: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

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

    // ========== TAB 4: NHẬN TRẢ ==========

    private void LoadCheckedOutData()
    {
        _context.ChangeTracker.Clear();

        var checkouts = _context.Checkouts
            .AsNoTracking()
            .Include(c => c.BorrowRequest)
                .ThenInclude(r => r.Requester)
            .Include(c => c.CheckedOutToUser)
            .Include(c => c.CheckoutItems)
                .ThenInclude(ci => ci.Asset)
            .Where(c => c.CheckoutItems.Any(ci => !ci.IsReturned))
            .ToList();

        var displayItems = checkouts.Select(c => new CheckoutDisplayItem { Checkout = c }).ToList();
        GridCheckedOut.ItemsSource = displayItems;

        // Đếm đơn quá hạn và hiển thị cảnh báo tổng
        var overdueCount = displayItems.Count(d => d.IsOverdue1Hour);
        var overdueNormal = displayItems.Count(d => d.IsOverdueNormal);
        if (overdueCount > 0)
        {
            TxtOverdueWarning.Text = $"⛔ CÓ {overdueCount} ĐƠN QUÁ HẠN TRÊN 1 GIỜ! Cần xử lý ngay!";
        }
        else if (overdueNormal > 0)
        {
            TxtOverdueWarning.Text = $"⚠️ Có {overdueNormal} đơn quá hạn trả.";
            TxtOverdueWarning.Foreground = System.Windows.Media.Brushes.OrangeRed;
        }
        else
        {
            TxtOverdueWarning.Text = "";
        }

        // Gửi thông báo cho các đơn quá hạn
        _ = SendOverdueNotificationsAsync(checkouts);
    }

    private async Task SendOverdueNotificationsAsync(List<Checkout> checkouts)
    {
        foreach (var checkout in checkouts)
        {
            if (checkout.BorrowRequest.ExpectedReturnAt < DateTime.Now)
            {
                var overtime = DateTime.Now - checkout.BorrowRequest.ExpectedReturnAt;
                string severity = overtime.TotalHours >= 1 ? "⛔ KHẨN CẤP" : "⚠️ Cảnh báo";
                string msg = $"{severity}: Đơn {checkout.BorrowRequest.RequestCode} ({checkout.CheckoutCode}) đã quá hạn trả {(int)overtime.TotalMinutes} phút!";

                // Thông báo cho người mượn
                await _notificationService.CreateNotificationAsync(
                    checkout.CheckedOutTo,
                    $"{severity}: Đơn mượn {checkout.BorrowRequest.RequestCode} quá hạn trả!",
                    $"Vui lòng trả CSVC ngay. Đã quá hạn {(int)overtime.TotalMinutes} phút.");

                // Thông báo cho quản lý đang đăng nhập
                var currentUser = _authService.CurrentUser;
                if (currentUser != null && currentUser.UserId != checkout.CheckedOutTo)
                {
                    await _notificationService.CreateNotificationAsync(
                        currentUser.UserId,
                        $"{severity}: Đơn {checkout.BorrowRequest.RequestCode} quá hạn!",
                        msg);
                }
            }
        }
    }

    private void GridCheckedOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridCheckedOut.SelectedItem is CheckoutDisplayItem displayItem)
        {
            _selectedCheckout = displayItem.Checkout;

            var items = _selectedCheckout.CheckoutItems
                .Where(ci => !ci.IsReturned)
                .Select(ci => new ReturnItemUI
                {
                    CheckoutItemId = ci.CheckoutItemId,
                    Asset = ci.Asset,
                    Quantity = ci.Quantity,
                    QuantityReturned = ci.Quantity,
                    ConditionAfter = "Tốt"
                }).ToList();

            GridChiTietTra.ItemsSource = items;
            BtnHoanTatTra.IsEnabled = true;
            BtnInPhieuTra.IsEnabled = false;
        }
    }

    private async void BtnHoanTatTra_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCheckout == null) return;

        try
        {
            var uiItems = GridChiTietTra.ItemsSource as List<ReturnItemUI>;
            if (uiItems == null) return;

            var dtoList = uiItems.Select(ui => new ReturnItemDto
            {
                CheckoutItemId = ui.CheckoutItemId,
                AssetId = ui.Asset.AssetId,
                QuantityReturned = ui.QuantityReturned,
                ConditionAfter = ui.ConditionAfter,
                IsDamaged = ui.ConditionAfter == "Hỏng",
                IsLost = ui.ConditionAfter == "Mất",
                DamageNote = ui.DamageNote
            }).ToList();

            var currentUserId = _authService.CurrentUser?.UserId ?? 1;
            var returnObj = await _returnService.CreateReturnAsync(
                _selectedCheckout.CheckoutId, currentUserId, TxtGhiChuTra.Text, dtoList);
            _lastReturnId = returnObj.ReturnId;

            bool hasDamage = dtoList.Any(d => d.IsDamaged || d.IsLost);
            if (hasDamage)
            {
                MessageBox.Show(
                    "Phát hiện có thiết bị hỏng/mất! Hệ thống đã tạm giữ đơn.\nVui lòng sang mục 'Sự cố' để lập biên bản.",
                    "Cảnh báo sự cố", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("Nhận trả CSVC thành công! Tồn kho đã được cộng lại.",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            BtnInPhieuTra.IsEnabled = true;
            BtnHoanTatTra.IsEnabled = false;
            GridChiTietTra.ItemsSource = null;
            LoadCheckedOutData();
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi nhận trả: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnInPhieuTra_Click(object sender, RoutedEventArgs e)
    {
        if (_lastReturnId == 0) return;

        try
        {
            var pdfBytes = await _reportService.GenerateReturnPdfAsync(_lastReturnId);
            
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"PhieuNhanTra_{_lastReturnId}.pdf");
            File.WriteAllBytes(path, pdfBytes);
            
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi in phiếu PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

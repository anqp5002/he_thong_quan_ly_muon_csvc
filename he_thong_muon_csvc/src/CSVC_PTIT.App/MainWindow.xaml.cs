using System.Windows;
using System.Windows.Controls;
using CSVC_PTIT.App.ViewModels;
using CSVC_PTIT.App.Views;
using CSVC_PTIT.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App;

/// <summary>
/// MainWindow — Khung giao diện chính (Navigation Shell).
/// Sidebar bên trái chứa menu, bên phải hiển thị UserControl tương ứng.
/// </summary>
public partial class MainWindow : Window
{
    private readonly IAuthService _authService;

    public MainWindow()
    {
        InitializeComponent();
        _authService = App.ServiceProvider.GetRequiredService<IAuthService>();
        
        SetupAuthUI();
        LoadNotificationCountAsync();

        // Mặc định hiển thị Dashboard khi app mở
        MainContent.Content = new DashboardView();
    }

    private void SetupAuthUI()
    {
        var user = _authService.CurrentUser;
        if (user == null) return;

        TxtUserName.Text = user.FullName;
        TxtUserRole.Text = user.Role?.RoleName ?? "Chưa rõ";

        var roleCode = user.Role?.RoleCode;

        foreach (var item in MenuList.Items)
        {
            if (item is ListBoxItem listBoxItem)
            {
                var tag = listBoxItem.Tag?.ToString();

                // Mặc định ẩn các menu đặc quyền
                if (tag == "DuyetDon" || tag == "DonDaDuyet")
                    listBoxItem.Visibility = Visibility.Collapsed;

                if (roleCode == "SV" || roleCode == "DT")
                {
                    // SV/DT: Chỉ xem Tổng quan, Tra cứu, Đơn mượn, Sự cố
                    if (tag == "Users" || tag == "Settings" || tag == "AuditLog" 
                        || tag == "Assets" || tag == "Checkout")
                    {
                        listBoxItem.Visibility = Visibility.Collapsed;
                    }
                }
                else if (roleCode == "QL")
                {
                    // QL: Xem Tổng quan, CSVC, Tra cứu, Duyệt đơn, Đơn đã duyệt, Bàn giao, Sự cố
                    if (tag == "Users" || tag == "Settings" || tag == "AuditLog")
                    {
                        listBoxItem.Visibility = Visibility.Collapsed;
                    }
                    // Mở 2 menu đặc quyền cho QL
                    if (tag == "DuyetDon" || tag == "DonDaDuyet")
                    {
                        listBoxItem.Visibility = Visibility.Visible;
                    }
                }
                // AD (Admin): Thấy tất cả trừ DuyetDon/DonDaDuyet (giữ Collapsed mặc định)
            }
        }
    }

    private async void LoadNotificationCountAsync()
    {
        try
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            using var scope = App.ServiceProvider.CreateScope();
            var notifService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var count = await notifService.GetUnreadCountAsync(user.UserId);

            NotificationBadge.Badge = count > 0 ? count.ToString() : null;
        }
        catch { /* Bỏ qua lỗi khi khởi tạo */ }
    }

    /// <summary>
    /// Xử lý khi user click vào menu item trên sidebar.
    /// </summary>
    private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TxtPageTitle is null || MainContent is null)
            return;

        if (MenuList.SelectedItem is not ListBoxItem selectedItem)
            return;

        var tag = selectedItem.Tag?.ToString();

        TxtPageTitle.Text = tag switch
        {
            "Dashboard" => "Tổng quan",
            "Users" => "Quản lý tài khoản",
            "Assets" => "Danh mục CSVC",
            "BorrowRequests" => "Đơn mượn",
            "Checkout" => "Bàn giao / Nhận trả",
            "Incidents" => "Sự cố",
            "Settings" => "Cấu hình hệ thống",
            "AuditLog" => "Nhật ký hoạt động",
            "TraCuu" => "Tra cứu CSVC",
            "DuyetDon" => "Duyệt đơn mượn",
            "DonDaDuyet" => "Đơn mượn đã duyệt",
            _ => "Tổng quan"
        };

        MainContent.Content = tag switch
        {
            "Dashboard" => new DashboardView(),
            "Users" => new QuanLyTaiKhoanView(),
            "Assets" => new DanhMucCSVCView(),
            "Settings" => new CauHinhHeThongView(),
            "AuditLog" => new NhatKyView(),
            "TraCuu" => new Views.SV.TraCuuCSVCView(),
            "BorrowRequests" => new Views.SV.TheoDoiDonMuonView(),
            "Checkout" => new CSVC_PTIT.App.Views.QL.BanGiaoCSVCView(),
            "Incidents" => new CSVC_PTIT.App.Views.QL.BienBanSuCoView(),
            "DuyetDon" => new CSVC_PTIT.App.Views.DT.DanhSachDonCanDuyetView(),
            "DonDaDuyet" => new CSVC_PTIT.App.Views.QL.DanhSachDonDaDuyetView(),
            _ => new PlaceholderView(TxtPageTitle.Text)
        };
    }

    /// <summary>
    /// Xử lý khi bấm nút chuông Thông báo.
    /// </summary>
    private void BtnNotification_Click(object sender, RoutedEventArgs e)
    {
        TxtPageTitle.Text = "Thông báo";
        MainContent.Content = new Views.ThongBaoView();
        MenuList.SelectedIndex = -1; // Bỏ chọn menu sidebar
    }

    private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
    {
        var vm = App.ServiceProvider.GetRequiredService<ChangePasswordViewModel>();
        var dialog = new ChangePasswordView(vm)
        {
            Owner = this
        };
        dialog.ShowDialog();
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Bạn có chắc muốn đăng xuất?",
            "Xác nhận",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _authService.Logout();
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            var loginVm = App.ServiceProvider.GetRequiredService<LoginViewModel>();
            var loginView = new LoginView(loginVm);
            
            this.Close();
            
            if (loginView.ShowDialog() == true)
            {
                var newMainWindow = new MainWindow();
                Application.Current.MainWindow = newMainWindow;
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                newMainWindow.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
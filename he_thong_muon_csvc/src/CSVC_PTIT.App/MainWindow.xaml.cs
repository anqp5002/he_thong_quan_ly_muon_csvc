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

        // Mặc định hiển thị Dashboard khi app mở
        MainContent.Content = new DashboardView();
    }

    private void SetupAuthUI()
    {
        var user = _authService.CurrentUser;
        if (user == null) return;

        TxtUserName.Text = user.FullName;
        TxtUserRole.Text = user.Role?.RoleName ?? "Chưa rõ";

        // Logic ẩn/hiện menu dựa vào role (tạm ẩn các item bằng cách duyệt tag)
        var roleCode = user.Role?.RoleCode;

        foreach (var item in MenuList.Items)
        {
            if (item is ListBoxItem listBoxItem)
            {
                var tag = listBoxItem.Tag?.ToString();
                if (roleCode == "SV" || roleCode == "DT") // Sinh viên, Đoàn thể
                {
                    // Chỉ xem Tổng quan, Đơn mượn, Sự cố
                    if (tag == "Users" || tag == "Settings" || tag == "AuditLog" || tag == "Assets" || tag == "Checkout")
                    {
                        listBoxItem.Visibility = Visibility.Collapsed;
                    }
                }
                else if (roleCode == "QL") // Quản lý
                {
                    // Không xem User, Config, AuditLog
                    if (tag == "Users" || tag == "Settings" || tag == "AuditLog")
                    {
                        listBoxItem.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Xử lý khi user click vào menu item trên sidebar.
    /// Đọc Tag của item được chọn → tạo UserControl tương ứng → gán vào ContentControl.
    /// </summary>
    private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Guard: XAML kích hoạt sự kiện này trước khi InitializeComponent() hoàn tất
        if (TxtPageTitle == null || MainContent == null)
            return;

        if (MenuList?.SelectedItem is not ListBoxItem selectedItem)
            return;

        var tag = selectedItem.Tag?.ToString();

        // Đổi tiêu đề trang trên top bar
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
            _ => "Tổng quan"
        };

        // Đổi nội dung vùng Content bên phải
        MainContent.Content = tag switch
        {
            "Dashboard" => new DashboardView(),
            "Users" => new QuanLyTaiKhoanView(),
            "Assets" => new DanhMucCSVCView(),
            "Settings" => new CauHinhHeThongView(),
            "AuditLog" => new NhatKyView(),
            "Checkout" => new CSVC_PTIT.App.Views.QL.BanGiaoCSVCView(),
            "Incidents" => new CSVC_PTIT.App.Views.QL.BienBanSuCoView(),
            "BaoCao" => new CSVC_PTIT.App.Views.QL.BaoCaoTonKhoView(),
            "DoiSoat" => new CSVC_PTIT.App.Views.QL.DoiSoatCSVCView(),
            _ => new PlaceholderView(TxtPageTitle.Text)
        };
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

    /// <summary>
    /// Xử lý nút Đăng xuất.
    /// </summary>
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
            
            var loginVm = App.ServiceProvider.GetRequiredService<LoginViewModel>();
            var loginView = new LoginView(loginVm);
            loginView.Show();
            
            this.Close();
        }
    }
}
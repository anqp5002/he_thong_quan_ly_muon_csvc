using System.Windows;
using System.Windows.Controls;
using CSVC_PTIT.App.Views;

namespace CSVC_PTIT.App;

/// <summary>
/// MainWindow — Khung giao diện chính (Navigation Shell).
/// Sidebar bên trái chứa menu, bên phải hiển thị UserControl tương ứng.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Mặc định hiển thị Dashboard khi app mở
        MainContent.Content = new DashboardView();
    }

    /// <summary>
    /// Xử lý khi user click vào menu item trên sidebar.
    /// Đọc Tag của item được chọn → tạo UserControl tương ứng → gán vào ContentControl.
    /// </summary>
    private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MenuList?.SelectedItem is not ListBoxItem selectedItem)
            return;

        if (TxtPageTitle == null || MainContent == null)
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
            // Các view khác sẽ thêm ở Sprint 1:
            // "Users" => new QuanLyTaiKhoanView(),
            // "Assets" => new DanhMucCSVCView(),
            _ => new PlaceholderView(TxtPageTitle.Text)
        };
    }

    /// <summary>
    /// Xử lý nút Đăng xuất.
    /// Sprint 1 sẽ thêm logic: clear session, quay về LoginView.
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
            // TODO Sprint 1: AuthService.Logout() + mở LoginView
            MessageBox.Show("Chức năng đăng xuất sẽ được hoàn thiện ở Sprint 1.",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
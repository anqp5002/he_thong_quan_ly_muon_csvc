using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CSVC_PTIT.App.ViewModels;

/// <summary>
/// ViewModel cho MainWindow — quản lý navigation, user info, sidebar.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    /// <summary>Tiêu đề trang hiện tại (hiển thị trên top bar)</summary>
    [ObservableProperty]
    private string _pageTitle = "Tổng quan";

    /// <summary>Tên user đang đăng nhập</summary>
    [ObservableProperty]
    private string _userName = "Admin";

    /// <summary>Vai trò hiển thị</summary>
    [ObservableProperty]
    private string _userRole = "Quản trị viên";
}

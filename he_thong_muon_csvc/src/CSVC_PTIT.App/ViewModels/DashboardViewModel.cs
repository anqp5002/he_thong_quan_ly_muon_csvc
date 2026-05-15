using CommunityToolkit.Mvvm.ComponentModel;

namespace CSVC_PTIT.App.ViewModels;

/// <summary>
/// ViewModel cho DashboardView — chứa số liệu thống kê tổng quan.
/// Sprint 1 sẽ kết nối DB thật để lấy số liệu.
/// </summary>
public partial class DashboardViewModel : ViewModelBase
{
    /// <summary>Tổng số CSVC trong hệ thống</summary>
    [ObservableProperty]
    private int _totalAssets = 10;

    /// <summary>Tổng số phòng học</summary>
    [ObservableProperty]
    private int _totalRooms = 5;

    /// <summary>Số đơn mượn đang chờ duyệt</summary>
    [ObservableProperty]
    private int _pendingRequests = 0;

    /// <summary>Tổng số tài khoản</summary>
    [ObservableProperty]
    private int _totalUsers = 10;
}

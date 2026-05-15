using CommunityToolkit.Mvvm.ComponentModel;

namespace CSVC_PTIT.App.ViewModels;

/// <summary>
/// Base class cho tất cả ViewModel trong app.
/// Kế thừa ObservableObject → tự thông báo View khi dữ liệu thay đổi.
/// Chứa các property dùng chung: IsLoading, ErrorMessage.
/// </summary>
public partial class ViewModelBase : ObservableObject
{
    /// <summary>Trạng thái đang tải dữ liệu (hiện spinner)</summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>Thông báo lỗi (nếu có)</summary>
    [ObservableProperty]
    private string? _errorMessage;
}

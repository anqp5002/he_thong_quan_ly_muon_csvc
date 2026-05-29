using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;

namespace CSVC_PTIT.App.ViewModels.DT;

public partial class ThongBaoKetQuaDuyetViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;

    [ObservableProperty]
    private ObservableCollection<BorrowRequest> _danhSachThongBao = new();

    [ObservableProperty]
    private bool _dangTai;

    private readonly int _currentUserId = 1;

    public ThongBaoKetQuaDuyetViewModel(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    [RelayCommand]
    private async Task TaiThongBaoAsync()
    {
        DangTai = true;
        var list = await _borrowService.GetRequestsByUserAsync(_currentUserId);

        // Chỉ hiện đơn đã được duyệt hoặc từ chối
        var daXuLy = list.Where(r =>
            r.Status == RequestStatus.Approved ||
            r.Status == RequestStatus.Rejected).ToList();

        DanhSachThongBao = new ObservableCollection<BorrowRequest>(daXuLy);
        DangTai = false;
    }
}
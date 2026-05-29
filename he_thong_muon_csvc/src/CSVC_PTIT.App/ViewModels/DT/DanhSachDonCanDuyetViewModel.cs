using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;

namespace CSVC_PTIT.App.ViewModels.DT;

public partial class DanhSachDonCanDuyetViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;

    [ObservableProperty]
    private ObservableCollection<BorrowRequest> _danhSachDon = new();

    [ObservableProperty]
    private BorrowRequest? _donDuocChon;

    [ObservableProperty]
    private bool _dangTai;

    private readonly int _currentUserId = 1;

    public DanhSachDonCanDuyetViewModel(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    [RelayCommand]
    private async Task TaiDanhSachAsync()
    {
        DangTai = true;
        var list = await _borrowService.GetRequestsByUserAsync(_currentUserId);

        // Chỉ hiện đơn đang chờ duyệt
        var choDuyet = list.Where(r => r.Status == RequestStatus.Pending).ToList();
        DanhSachDon = new ObservableCollection<BorrowRequest>(choDuyet);
        DangTai = false;
    }
}
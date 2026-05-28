using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.App.ViewModels.SV;

public partial class TheoDoiDonMuonViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;

    [ObservableProperty]
    private ObservableCollection<BorrowRequest> _danhSachDon = new();

    [ObservableProperty]
    private BorrowRequest? _donDuocChon;

    [ObservableProperty]
    private bool _dangTai;

    // Tạm thời hardcode userId = 1 để test, sau này sẽ lấy từ session đăng nhập
    private readonly int _currentUserId = 1;

    public TheoDoiDonMuonViewModel(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    [RelayCommand]
    private async Task TaiDanhSachAsync()
    {
        DangTai = true;
        var list = await _borrowService.GetRequestsByUserAsync(_currentUserId);
        DanhSachDon = new ObservableCollection<BorrowRequest>(list);
        DangTai = false;
    }
}
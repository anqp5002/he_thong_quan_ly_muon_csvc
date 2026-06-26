using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.App.ViewModels.SV;

public partial class TheoDoiDonMuonViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private ObservableCollection<BorrowRequest> _danhSachDon = new();

    [ObservableProperty]
    private BorrowRequest? _donDuocChon;

    [ObservableProperty]
    private bool _dangTai;

    public TheoDoiDonMuonViewModel(IBorrowService borrowService, IAuthService authService)
    {
        _borrowService = borrowService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task TaiDanhSachAsync()
    {
        var user = _authService.CurrentUser;
        if (user == null) return;

        DangTai = true;
        var list = await _borrowService.GetRequestsByUserAsync(user.UserId);
        DanhSachDon = new ObservableCollection<BorrowRequest>(list);
        DangTai = false;
    }

    [RelayCommand]
    private void TaoDonMoi()
    {
        var dialog = new CSVC_PTIT.App.Views.SV.DangKyMuonView();
        dialog.ShowDialog();
        // Sau khi đóng dialog, có thể tải lại danh sách
        _ = TaiDanhSachAsync();
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;

namespace CSVC_PTIT.App.ViewModels.DT;

public partial class DuyetDonViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;

    [ObservableProperty]
    private BorrowRequest? _donHienTai;

    [ObservableProperty]
    private string _lyDoTuChoi = string.Empty;

    [ObservableProperty]
    private string _thongBao = string.Empty;

    [ObservableProperty]
    private bool _dangXuLy;

    public DuyetDonViewModel(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    public void LoadDon(BorrowRequest don)
    {
        DonHienTai = don;
    }

    [RelayCommand]
    private async Task DuyetDonAsync()
    {
        if (DonHienTai == null) return;

        DangXuLy = true;
        await _borrowService.DuyetDonAsync(DonHienTai.RequestId);
        ThongBao = $"Đã duyệt đơn {DonHienTai.RequestCode}!";
        DangXuLy = false;
    }

    [RelayCommand]
    private async Task TuChoiDonAsync()
    {
        if (DonHienTai == null) return;

        if (string.IsNullOrWhiteSpace(LyDoTuChoi))
        {
            ThongBao = "Vui lòng nhập lý do từ chối!";
            return;
        }

        DangXuLy = true;
        await _borrowService.TuChoiDonAsync(DonHienTai.RequestId, LyDoTuChoi);
        ThongBao = $"Đã từ chối đơn {DonHienTai.RequestCode}!";
        DangXuLy = false;
    }
}
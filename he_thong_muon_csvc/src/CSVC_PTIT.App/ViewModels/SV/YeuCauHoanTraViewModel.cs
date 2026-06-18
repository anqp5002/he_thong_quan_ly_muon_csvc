using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;

namespace CSVC_PTIT.App.ViewModels.SV;

public partial class YeuCauHoanTraViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;

    [ObservableProperty]
    private ObservableCollection<BorrowRequest> _danhSachDangMuon = new();

    [ObservableProperty]
    private BorrowRequest? _donDuocChon;

    [ObservableProperty]
    private string _ghiChuTinhTrang = string.Empty;

    [ObservableProperty]
    private string _thongBao = string.Empty;

    [ObservableProperty]
    private bool _dangXuLy;

    private readonly int _currentUserId = 1;

    public YeuCauHoanTraViewModel(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    [RelayCommand]
    private async Task TaiDanhSachAsync()
    {
        DangXuLy = true;
        var list = await _borrowService.GetRequestsByUserAsync(_currentUserId);

        // Chỉ hiện những đơn đang được bàn giao (CheckedOut)
        var dangMuon = list.Where(r => r.Status == RequestStatus.CheckedOut).ToList();
        DanhSachDangMuon = new ObservableCollection<BorrowRequest>(dangMuon);
        DangXuLy = false;
    }

    [RelayCommand]
    private async Task GuiYeuCauTraAsync()
    {
        if (DonDuocChon == null)
        {
            ThongBao = "Vui lòng chọn đơn mượn cần trả!";
            return;
        }

        DangXuLy = true;
        ThongBao = string.Empty;

        // Tạm thời chỉ cập nhật ghi chú, logic trả thật sẽ do Dev C làm (ReturnService)
        DonDuocChon.RequestNote = GhiChuTinhTrang;
        ThongBao = $"Đã gửi yêu cầu trả cho đơn {DonDuocChon.RequestCode}!";

        DangXuLy = false;
    }
}
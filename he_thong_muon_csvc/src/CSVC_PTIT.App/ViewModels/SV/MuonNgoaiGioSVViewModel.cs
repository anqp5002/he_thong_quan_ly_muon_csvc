using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.DTOs;
using CSVC_PTIT.Core.Interfaces;

namespace CSVC_PTIT.App.ViewModels.SV;

public partial class MuonNgoaiGioSVViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;

    // Thông tin sinh viên
    [ObservableProperty] private string _hoTen = string.Empty;
    [ObservableProperty] private string _mssv = string.Empty;
    [ObservableProperty] private string _soDienThoai = string.Empty;
    [ObservableProperty] private string _lop = string.Empty;

    // Thông tin mượn
    [ObservableProperty] private string _mucDich = string.Empty;
    [ObservableProperty] private string _diaDiem = string.Empty;
    [ObservableProperty] private string _ghiChu = string.Empty;

    // Thời gian
    [ObservableProperty] private DateTime _ngayMuon = DateTime.Today;
    [ObservableProperty] private DateTime _gioMuon = DateTime.Now;
    [ObservableProperty] private DateTime _gioTra = DateTime.Now.AddHours(2);

    // Danh sách CSVC
    [ObservableProperty]
    private ObservableCollection<BorrowRequestAssetDto> _danhSachCsvc = new();

    [ObservableProperty] private string _thongBao = string.Empty;
    [ObservableProperty] private bool _dangXuLy;

    private readonly int _currentUserId = 1;

    public MuonNgoaiGioSVViewModel(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    [RelayCommand]
    private void ThemCsvc()
    {
        DanhSachCsvc.Add(new BorrowRequestAssetDto
        {
            AssetId = 0,
            QuantityRequested = 1
        });
    }

    [RelayCommand]
    private void XoaCsvc(BorrowRequestAssetDto item)
    {
        DanhSachCsvc.Remove(item);
    }

    [RelayCommand]
    private async Task GuiDonAsync()
    {
        if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(Mssv))
        {
            ThongBao = "Vui lòng điền đầy đủ họ tên và MSSV!";
            return;
        }

        if (DanhSachCsvc.Count == 0)
        {
            ThongBao = "Vui lòng thêm ít nhất 1 CSVC!";
            return;
        }

        DangXuLy = true;
        ThongBao = string.Empty;

        var dto = new CreateBorrowRequestDto
        {
            RequesterId = _currentUserId,
            ContactPhone = SoDienThoai,
            Title = $"Mượn ngoài giờ - {HoTen}",
            Purpose = MucDich,
            RequestNote = GhiChu,
            BorrowStartAt = NgayMuon.Date + GioMuon.TimeOfDay,
            BorrowEndAt = NgayMuon.Date + GioTra.TimeOfDay,
            Assets = DanhSachCsvc.ToList()
        };

        await _borrowService.CreateOffHoursRequestAsync(dto);
        ThongBao = "Gửi đơn thành công! Vui lòng chờ duyệt.";
        DangXuLy = false;
    }
}
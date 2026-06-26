using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.DTOs;
using CSVC_PTIT.Core.Interfaces;
using System.Collections.ObjectModel;

namespace CSVC_PTIT.App.ViewModels.SV;

public partial class DangKyMuonViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;

    // Thông tin người mượn
    [ObservableProperty] private string _hoTen = string.Empty;
    [ObservableProperty] private string _mssv = string.Empty;
    [ObservableProperty] private string _soDienThoai = string.Empty;
    [ObservableProperty] private string _lop = string.Empty;
    [ObservableProperty] private string _monHoc = string.Empty;
    [ObservableProperty] private string _phongHoc = string.Empty;
    [ObservableProperty] private string _ghiChu = string.Empty;

    // Thời gian
    [ObservableProperty] private DateTime _ngayMuon = DateTime.Today;
    [ObservableProperty] private DateTime _gioMuon = DateTime.Now;
    [ObservableProperty] private DateTime _gioTra = DateTime.Now.AddHours(2);

    // Danh sách CSVC trong đơn
    [ObservableProperty]
    private ObservableCollection<BorrowRequestAssetDto> _danhSachCsvc = new();

    // Thông báo kết quả
    [ObservableProperty] private string _thongBao = string.Empty;
    [ObservableProperty] private bool _dangXuLy;

    // Tạm hardcode userId = 1, sau này lấy từ session
    private readonly IAuthService _authService;
    private readonly IAssetService _assetService;

    [ObservableProperty]
    private ObservableCollection<CSVC_PTIT.Data.Entities.Asset> _availableAssets = new();

    public DangKyMuonViewModel(IBorrowService borrowService, IAuthService authService, IAssetService assetService)
    {
        _borrowService = borrowService;
        _authService = authService;
        _assetService = assetService;

        LoadAssetsAsync();
    }

    private async void LoadAssetsAsync()
    {
        var assets = await _assetService.GetAllAsync();
        // Chỉ lấy những tài sản khả dụng > 0
        AvailableAssets = new ObservableCollection<CSVC_PTIT.Data.Entities.Asset>(assets.Where(a => a.AvailableQuantity > 0));
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

        if (DanhSachCsvc.Any(x => x.AssetId == 0))
        {
            ThongBao = "Vui lòng chọn Tên CSVC hợp lệ trong bảng!";
            return;
        }

        DangXuLy = true;
        ThongBao = string.Empty;

        var user = _authService.CurrentUser;
        if (user == null) 
        {
            ThongBao = "Bạn chưa đăng nhập!";
            DangXuLy = false;
            return;
        }

        try
        {
            var dto = new CreateBorrowRequestDto
            {
                RequesterId = user.UserId,
                ContactPhone = SoDienThoai,
                Title = $"{MonHoc} - {PhongHoc}",
                Purpose = $"Mượn CSVC cho môn {MonHoc}",
                RequestNote = GhiChu,
                BorrowStartAt = NgayMuon.Date + GioMuon.TimeOfDay,
                BorrowEndAt = NgayMuon.Date + GioTra.TimeOfDay,
                Assets = DanhSachCsvc.ToList()
            };

            DonVuaTao = await _borrowService.CreateInClassRequestAsync(dto);
            ThongBao = $"Gửi đơn thành công! Mã đơn: {DonVuaTao?.RequestCode}. Bạn có thể xuất PDF ngay bây giờ.";
        }
        catch (Exception ex)
        {
            ThongBao = $"Lỗi: {ex.Message}";
        }
        finally
        {
            DangXuLy = false;
        }
    }
    // Đơn vừa tạo (dùng để xuất PDF)
    [ObservableProperty] private CSVC_PTIT.Data.Entities.BorrowRequest? _donVuaTao;

    [RelayCommand]
    private void XuatPdf()
    {
        if (DonVuaTao == null)
        {
            ThongBao = "Chưa có đơn để xuất PDF. Vui lòng gửi đơn trước!";
            return;
        }

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"PhieuMuon_{DonVuaTao.RequestCode}",
            DefaultExt = ".pdf",
            Filter = "PDF files (*.pdf)|*.pdf"
        };

        if (dialog.ShowDialog() == true)
        {
            CSVC_PTIT.Core.Services.PhieuMuonTrongGioPdf.GenerateToFile(DonVuaTao, dialog.FileName);
            ThongBao = $"Xuất PDF thành công: {dialog.FileName}";
        }
    }
}
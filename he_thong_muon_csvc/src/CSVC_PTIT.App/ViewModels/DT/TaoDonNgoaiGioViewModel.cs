using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.DTOs;
using CSVC_PTIT.Core.Interfaces;

namespace CSVC_PTIT.App.ViewModels.DT;

public partial class TaoDonNgoaiGioViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;
    private readonly IAuthService _authService;

    // Thông tin người gửi
    [ObservableProperty] private string _hoTen = string.Empty;
    [ObservableProperty] private string _chucVu = string.Empty;
    [ObservableProperty] private string _donVi = string.Empty;
    [ObservableProperty] private string _soDienThoai = string.Empty;

    // Thông tin sự kiện
    [ObservableProperty] private string _tenSuKien = string.Empty;
    [ObservableProperty] private string _mucDich = string.Empty;
    [ObservableProperty] private string _diaDiem = string.Empty;
    [ObservableProperty] private string _ghiChu = string.Empty;

    // Thời gian
    [ObservableProperty] private DateTime _ngayMuon = DateTime.Today;
    [ObservableProperty] private DateTime _gioMuon = DateTime.Now;
    [ObservableProperty] private DateTime _gioTra = DateTime.Now.AddHours(4);

    // Danh sách CSVC
    [ObservableProperty]
    private ObservableCollection<BorrowRequestAssetDto> _danhSachCsvc = new();

    [ObservableProperty] private string _thongBao = string.Empty;
    [ObservableProperty] private bool _dangXuLy;

    public TaoDonNgoaiGioViewModel(IBorrowService borrowService, IAuthService authService)
    {
        _borrowService = borrowService;
        _authService = authService;

        var user = _authService.CurrentUser;
        if (user != null)
        {
            HoTen = user.FullName;
            DonVi = user.Department?.DepartmentName ?? user.ClassName ?? string.Empty;
            SoDienThoai = user.Phone ?? string.Empty;
            ChucVu = user.BorrowerType.ToString();
        }
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
        if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(DonVi))
        {
            ThongBao = "Vui lòng điền đầy đủ họ tên và đơn vị!";
            return;
        }

        if (string.IsNullOrWhiteSpace(TenSuKien))
        {
            ThongBao = "Vui lòng điền tên sự kiện!";
            return;
        }

        if (DanhSachCsvc.Count == 0)
        {
            ThongBao = "Vui lòng thêm ít nhất 1 CSVC!";
            return;
        }

        if (DanhSachCsvc.Any(x => x.AssetId <= 0))
        {
            ThongBao = "Vui lòng nhập mã CSVC hợp lệ!";
            return;
        }

        if (DanhSachCsvc.Any(x => x.QuantityRequested <= 0))
        {
            ThongBao = "Số lượng CSVC phải lớn hơn 0!";
            return;
        }

        var user = _authService.CurrentUser;
        if (user == null)
        {
            ThongBao = "Bạn chưa đăng nhập!";
            return;
        }

        DangXuLy = true;
        ThongBao = string.Empty;

        try
        {
            var dto = new CreateBorrowRequestDto
            {
                RequesterId = user.UserId,
                ContactPhone = SoDienThoai,
                Title = TenSuKien,
                Purpose = string.IsNullOrWhiteSpace(MucDich) ? DiaDiem : $"{MucDich} - {DiaDiem}",
                RequestNote = GhiChu,
                BorrowStartAt = NgayMuon.Date + GioMuon.TimeOfDay,
                BorrowEndAt = NgayMuon.Date + GioTra.TimeOfDay,
                Assets = DanhSachCsvc.ToList()
            };

            DonVuaTao = await _borrowService.CreateOffHoursRequestAsync(dto);
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
            FileName = $"DonMuon_{DonVuaTao.RequestCode}",
            DefaultExt = ".pdf",
            Filter = "PDF files (*.pdf)|*.pdf"
        };

        if (dialog.ShowDialog() == true)
        {
            CSVC_PTIT.Core.Services.DonMuonNgoaiGioPdf.GenerateToFile(DonVuaTao, dialog.FileName);
            ThongBao = $"Xuất PDF thành công: {dialog.FileName}";
        }
    }
}

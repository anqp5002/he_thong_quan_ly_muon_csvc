using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.DTOs;
using CSVC_PTIT.Core.Interfaces;
using System.Collections.ObjectModel;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.ViewModels.SV;

public class RoomStatusItem
{
    public string RoomCode { get; set; } = string.Empty;
    public string TrangThaiSang { get; set; } = "Trống";
    public string TrangThaiChieu { get; set; } = "Trống";
    public string TrangThaiToi { get; set; } = "Trống";
}

public partial class DangKyMuonViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;

    // Thông tin người mượn
    [ObservableProperty] private string _hoTen = string.Empty;
    [ObservableProperty] private string _mssv = string.Empty;
    [ObservableProperty] private string _soDienThoai = string.Empty;
    [ObservableProperty] private string _lop = string.Empty;
    [ObservableProperty] private string _monHoc = string.Empty;
    [ObservableProperty] private string _ghiChu = string.Empty;

    [ObservableProperty] private Room? _selectedRoom;
    [ObservableProperty] private ObservableCollection<Room> _availableRooms = new();

    // Thời gian
    [ObservableProperty] private DateTime _ngayMuon = DateTime.Today;
    [ObservableProperty] private DateTime _gioMuon = DateTime.Now;
    [ObservableProperty] private DateTime _gioTra = DateTime.Now.AddHours(2);

    // Lịch phòng
    [ObservableProperty] private DateTime _ngayTraCuu = DateTime.Today;
    [ObservableProperty] private ObservableCollection<RoomStatusItem> _danhSachLichPhong = new();

    // Danh sách CSVC trong đơn
    [ObservableProperty]
    private ObservableCollection<BorrowRequestAssetDto> _danhSachCsvc = new();

    // Thông báo kết quả
    [ObservableProperty] private string _thongBao = string.Empty;
    [ObservableProperty] private bool _dangXuLy;

    // Tạm hardcode userId = 1, sau này lấy từ session
    private readonly IAuthService _authService;
    private readonly IAssetService _assetService;
    private readonly CsvcDbContext _context;

    [ObservableProperty]
    private ObservableCollection<Asset> _availableAssets = new();

    public DangKyMuonViewModel(IBorrowService borrowService, IAuthService authService, IAssetService assetService)
    {
        _borrowService = borrowService;
        _authService = authService;
        _assetService = assetService;
        _context = App.ServiceProvider.GetRequiredService<CsvcDbContext>();

        // Tự động điền thông tin từ tài khoản đăng nhập
        var user = _authService.CurrentUser;
        if (user != null)
        {
            HoTen = user.FullName;
            Mssv = user.StudentCode ?? user.EmployeeCode ?? "";
            Lop = user.ClassName ?? user.Department?.DepartmentName ?? "";
            SoDienThoai = user.Phone ?? "";
        }

        LoadAssetsAsync();
        _ = TaiLichPhongAsync();
    }

    private async void LoadAssetsAsync()
    {
        var assets = await _assetService.GetAllAsync();
        // Chỉ lấy những tài sản khả dụng > 0
        AvailableAssets = new ObservableCollection<Asset>(assets.Where(a => a.AvailableQuantity > 0));

        // Load danh sách phòng học
        var rooms = await _context.Rooms.OrderBy(r => r.RoomCode).ToListAsync();
        AvailableRooms = new ObservableCollection<Room>(rooms);
    }

    [RelayCommand]
    private async Task TaiLichPhongAsync()
    {
        var targetDate = NgayTraCuu.Date;
        var endOfDay = targetDate.AddDays(1).AddTicks(-1);

        var rooms = await _context.Rooms.OrderBy(r => r.RoomCode).ToListAsync();
        var requests = await _context.BorrowRequests
            .Include(r => r.BorrowRequestRooms)
            .Where(r => r.Status == RequestStatus.Approved || r.Status == RequestStatus.CheckedOut)
            .Where(r => r.BorrowStartAt <= endOfDay && r.BorrowEndAt >= targetDate)
            .ToListAsync();

        var list = new ObservableCollection<RoomStatusItem>();
        foreach (var room in rooms)
        {
            var item = new RoomStatusItem { RoomCode = room.RoomCode };
            
            // Tìm các đơn mượn phòng này trong ngày
            var roomRequests = requests.Where(r => r.BorrowRequestRooms.Any(rr => rr.RoomId == room.RoomId)).ToList();
            
            foreach (var req in roomRequests)
            {
                var start = req.BorrowStartAt.TimeOfDay;
                var end = req.BorrowEndAt.TimeOfDay;

                if (start < new TimeSpan(11, 30, 0)) item.TrangThaiSang = $"Kín ({req.BorrowStartAt:HH:mm}-{req.BorrowEndAt:HH:mm})";
                if (start < new TimeSpan(17, 30, 0) && end > new TimeSpan(12, 0, 0)) item.TrangThaiChieu = $"Kín ({req.BorrowStartAt:HH:mm}-{req.BorrowEndAt:HH:mm})";
                if (end > new TimeSpan(17, 30, 0)) item.TrangThaiToi = $"Kín ({req.BorrowStartAt:HH:mm}-{req.BorrowEndAt:HH:mm})";
            }
            list.Add(item);
        }
        DanhSachLichPhong = list;
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
            // Cập nhật thông tin profile nếu người dùng có sửa trên form
            await _authService.UpdateProfileAsync(user.UserId, HoTen, Mssv, Lop, SoDienThoai);

            var dto = new CreateBorrowRequestDto
            {
                RequesterId = user.UserId,
                ContactPhone = SoDienThoai,
                Title = SelectedRoom != null ? $"{MonHoc} - {SelectedRoom.RoomCode}" : $"{MonHoc}",
                Purpose = $"Mượn CSVC cho môn {MonHoc}",
                RequestNote = GhiChu,
                RoomId = SelectedRoom?.RoomId,
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
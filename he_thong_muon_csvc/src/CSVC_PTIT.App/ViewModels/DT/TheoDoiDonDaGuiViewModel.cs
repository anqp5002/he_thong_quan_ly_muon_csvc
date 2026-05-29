using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;

namespace CSVC_PTIT.App.ViewModels.DT;

public partial class TheoDoiDonDaGuiViewModel : ObservableObject
{
    private readonly IBorrowService _borrowService;

    [ObservableProperty]
    private ObservableCollection<BorrowRequest> _danhSachDon = new();

    [ObservableProperty]
    private BorrowRequest? _donDuocChon;

    [ObservableProperty]
    private bool _dangTai;

    private readonly int _currentUserId = 1;

    public TheoDoiDonDaGuiViewModel(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    [RelayCommand]
    private async Task TaiDanhSachAsync()
    {
        DangTai = true;
        var list = await _borrowService.GetRequestsByUserAsync(_currentUserId);

        // Chỉ hiện đơn ngoài giờ
        var ngoaiGio = list.Where(r => r.RequestType == RequestType.OffHours).ToList();
        DanhSachDon = new ObservableCollection<BorrowRequest>(ngoaiGio);
        DangTai = false;
    }
}
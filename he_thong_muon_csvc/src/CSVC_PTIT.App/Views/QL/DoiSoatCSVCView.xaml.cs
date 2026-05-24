using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views.QL;

public class ReturnItemViewModel
{
    public int CheckoutItemId { get; set; }
    public Asset Asset { get; set; } = null!;
    public int Quantity { get; set; } // Số lượng đã giao
    public int QuantityReturned { get; set; } // Số lượng thực tế trả
    public string ConditionAfter { get; set; } = "Tốt";
    public string? DamageNote { get; set; }
}

public partial class DoiSoatCSVCView : UserControl
{
    private readonly IReturnService _returnService;
    private readonly IReportService _reportService;
    private readonly CsvcDbContext _context;

    private int _lastReturnId = 0;

    public DoiSoatCSVCView()
    {
        InitializeComponent();
        
        _returnService = App.ServiceProvider.GetRequiredService<IReturnService>();
        _reportService = App.ServiceProvider.GetRequiredService<IReportService>();
        _context = App.ServiceProvider.GetRequiredService<CsvcDbContext>();

        LoadData();
    }

    private void LoadData()
    {
        // Lấy danh sách các phiếu bàn giao chưa trả hết
        var checkouts = _context.Checkouts
            .Include(c => c.CheckoutItems)
            .ThenInclude(ci => ci.Asset)
            .Where(c => c.CheckoutItems.Any(ci => !ci.IsReturned))
            .ToList();
        
        CbbPhieuBanGiao.ItemsSource = checkouts;
    }

    private void CbbPhieuBanGiao_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CbbPhieuBanGiao.SelectedItem is Checkout selected)
        {
            var items = selected.CheckoutItems.Where(ci => !ci.IsReturned).Select(ci => new ReturnItemViewModel
            {
                CheckoutItemId = ci.CheckoutItemId,
                Asset = ci.Asset,
                Quantity = ci.Quantity,
                QuantityReturned = ci.Quantity, // Mặc định là trả đủ
                ConditionAfter = "Tốt"
            }).ToList();

            GridChiTietTra.ItemsSource = items;
            BtnInPhieuTra.IsEnabled = false;
            BtnHoanTatTra.IsEnabled = true;
        }
    }

    private async void BtnHoanTatTra_Click(object sender, RoutedEventArgs e)
    {
        if (CbbPhieuBanGiao.SelectedItem is Checkout selected)
        {
            try
            {
                var uiItems = GridChiTietTra.ItemsSource as List<ReturnItemViewModel>;
                if (uiItems == null) return;

                var dtoList = uiItems.Select(ui => new ReturnItemDto
                {
                    CheckoutItemId = ui.CheckoutItemId,
                    AssetId = ui.Asset.AssetId,
                    QuantityReturned = ui.QuantityReturned,
                    ConditionAfter = ui.ConditionAfter,
                    IsDamaged = ui.ConditionAfter == "Hỏng",
                    IsLost = ui.ConditionAfter == "Mất",
                    DamageNote = ui.DamageNote
                }).ToList();

                var returnObj = await _returnService.CreateReturnAsync(selected.CheckoutId, 1, TxtGhiChuTra.Text, dtoList);
                _lastReturnId = returnObj.ReturnId;

                MessageBox.Show("Đã nhận trả CSVC thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                
                BtnInPhieuTra.IsEnabled = true;
                BtnHoanTatTra.IsEnabled = false;
                LoadData();
                GridChiTietTra.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nhận trả: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void BtnInPhieuTra_Click(object sender, RoutedEventArgs e)
    {
        if (_lastReturnId == 0) return;

        try
        {
            var pdfBytes = await _reportService.GenerateReturnPdfAsync(_lastReturnId);
            
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"PhieuNhanTra_{_lastReturnId}.pdf");
            File.WriteAllBytes(path, pdfBytes);
            
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi in phiếu PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

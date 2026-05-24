using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views.QL;

public partial class BanGiaoCSVCView : UserControl
{
    private readonly ICheckoutService _checkoutService;
    private readonly IReportService _reportService;
    private readonly CsvcDbContext _context;

    public BanGiaoCSVCView()
    {
        InitializeComponent();
        
        // Resolve services from DI Container
        _checkoutService = App.ServiceProvider.GetRequiredService<ICheckoutService>();
        _reportService = App.ServiceProvider.GetRequiredService<IReportService>();
        _context = App.ServiceProvider.GetRequiredService<CsvcDbContext>();

        LoadData();
    }

    private void LoadData()
    {
        var requests = _context.BorrowRequests
            .Include(r => r.Requester)
            .Where(r => r.Status == RequestStatus.Approved || r.Status == RequestStatus.CheckedOut)
            .ToList();
        
        GridDonMuon.ItemsSource = requests;
    }

    private async void BtnCreateCheckout_Click(object sender, RoutedEventArgs e)
    {
        if (GridDonMuon.SelectedItem is CSVC_PTIT.Data.Entities.BorrowRequest selected)
        {
            try
            {
                if (selected.Status == RequestStatus.CheckedOut)
                {
                    MessageBox.Show("Đơn này đã được bàn giao rồi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Call Core Service (Sprint 1)
                var conditions = new Dictionary<int, string>(); // Default conditions
                await _checkoutService.CreateCheckoutAsync(selected.RequestId, 1 /* Admin ID */, "Bàn giao tự động", conditions);
                
                MessageBox.Show("Tạo phiếu bàn giao thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn một đơn mượn để bàn giao.", "Chú ý", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void BtnExportPdf_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Call Core Service (Sprint 3)
            // Cứ tạo báo cáo tồn kho để demo Sprint 3
            var pdfBytes = await _reportService.GenerateInventoryReportPdfAsync();
            
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BaoCaoTonKho.pdf");
            File.WriteAllBytes(path, pdfBytes);
            
            MessageBox.Show($"Đã xuất PDF thành công tại: {path}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Mở file PDF lên
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

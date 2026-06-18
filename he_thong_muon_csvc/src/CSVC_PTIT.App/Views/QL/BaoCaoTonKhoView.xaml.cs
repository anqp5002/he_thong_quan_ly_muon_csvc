using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views.QL;

public partial class BaoCaoTonKhoView : UserControl
{
    private readonly IReportService _reportService;
    private readonly CsvcDbContext _context;

    public BaoCaoTonKhoView()
    {
        InitializeComponent();
        
        _reportService = App.ServiceProvider.GetRequiredService<IReportService>();
        _context = App.ServiceProvider.GetRequiredService<CsvcDbContext>();

        LoadData();
    }

    private void LoadData()
    {
        GridAssets.ItemsSource = _context.Assets
            .Include(a => a.Category)
            .ToList();
    }

    private void BtnFilter_Click(object sender, RoutedEventArgs e)
    {
        LoadData(); // Chức năng demo, gọi lại load data
    }

    private async void BtnExportPdf_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var pdfBytes = await _reportService.GenerateInventoryReportPdfAsync();
            
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"BaoCaoTonKho_{DateTime.Now:yyyyMMdd}.pdf");
            File.WriteAllBytes(path, pdfBytes);
            
            MessageBox.Show($"Đã xuất Báo cáo tồn kho PDF thành công tại: {path}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views.QL;

public partial class BienBanSuCoView : UserControl
{
    private readonly IDamageReportService _damageReportService;
    private readonly IReportService _reportService;
    private readonly CsvcDbContext _context;
    
    private int _lastReportId = 0;

    public BienBanSuCoView()
    {
        InitializeComponent();
        
        _damageReportService = App.ServiceProvider.GetRequiredService<IDamageReportService>();
        _reportService = App.ServiceProvider.GetRequiredService<IReportService>();
        _context = App.ServiceProvider.GetRequiredService<CsvcDbContext>();

        LoadData();
    }

    private void LoadData()
    {
        CbbRequest.ItemsSource = _context.BorrowRequests.ToList();
        CbbAsset.ItemsSource = _context.Assets.ToList();
        CbbUser.ItemsSource = _context.Users.ToList();

        GridReports.ItemsSource = _context.DamageReports
            .Include(r => r.Asset)
            .ToList();
    }

    private async void BtnCreateReport_Click(object sender, RoutedEventArgs e)
    {
        if (CbbRequest.SelectedItem is BorrowRequest request && CbbAsset.SelectedItem is Asset asset && CbbUser.SelectedItem is User user)
        {
            try
            {
                var incidentTypeStr = (CbbIncidentType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Damage";
                var severityStr = (CbbSeverity.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Low";
                
                decimal.TryParse(TxtEstimatedCompensation.Text, out decimal compensation);

                var dto = new DamageReportDto
                {
                    RequestId = request.RequestId, // Lấy ID thực tế từ combobox
                    AssetId = asset.AssetId,
                    ResponsibleUserId = user.UserId,
                    IncidentType = Enum.Parse<IncidentType>(incidentTypeStr),
                    Severity = Enum.Parse<Severity>(severityStr),
                    Description = TxtDescription.Text,
                    EstimatedCompensation = compensation
                };

                var report = await _damageReportService.CreateReportAsync(1, dto); // 1 = Admin ID
                _lastReportId = report.ReportId;

                MessageBox.Show("Đã tạo biên bản sự cố thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                
                BtnPrintReport.IsEnabled = true;
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tạo biên bản: {ex.Message}{(ex.InnerException != null ? " (" + ex.InnerException.Message + ")" : "")}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Đơn mượn, Tài sản và Người chịu trách nhiệm.", "Chú ý", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void BtnPrintReport_Click(object sender, RoutedEventArgs e)
    {
        if (_lastReportId == 0) return;

        try
        {
            var pdfBytes = await _reportService.GenerateDamageReportPdfAsync(_lastReportId);
            
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"BienBanSuCo_{_lastReportId}.pdf");
            File.WriteAllBytes(path, pdfBytes);
            
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi in biên bản PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

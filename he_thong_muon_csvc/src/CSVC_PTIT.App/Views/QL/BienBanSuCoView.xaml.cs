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
    private readonly IAuthService _authService;
    private readonly CsvcDbContext _context;
    
    private int _lastReportId = 0;

    public BienBanSuCoView()
    {
        InitializeComponent();
        
        _damageReportService = App.ServiceProvider.GetRequiredService<IDamageReportService>();
        _reportService = App.ServiceProvider.GetRequiredService<IReportService>();
        _authService = App.ServiceProvider.GetRequiredService<IAuthService>();
        _context = App.ServiceProvider.GetRequiredService<CsvcDbContext>();

        LoadData();
    }

    private void LoadData()
    {
        // Lấy những đơn mượn đang hoạt động (Approved hoặc CheckedOut)
        var activeRequests = _context.BorrowRequests
            .Include(r => r.Requester)
            .Include(r => r.BorrowRequestAssets)
            .ThenInclude(ra => ra.Asset)
            .Where(r => r.Status == RequestStatus.Approved || r.Status == RequestStatus.CheckedOut)
            .ToList();
            
        CbbRequest.ItemsSource = activeRequests;

        GridReports.ItemsSource = _context.DamageReports
            .Include(r => r.Asset)
            .ToList();
    }

    private void CbbRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CbbRequest.SelectedItem is BorrowRequest request)
        {
            // Tự động gán người chịu trách nhiệm (Requester) và khóa lại
            CbbUser.ItemsSource = new List<User> { request.Requester };
            CbbUser.SelectedItem = request.Requester;
            CbbUser.IsEnabled = false;

            // Chỉ load những tài sản thuộc về đơn mượn này
            CbbAsset.ItemsSource = request.BorrowRequestAssets.Select(ra => ra.Asset).ToList();
        }
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

                var currentUserId = _authService.CurrentUser?.UserId ?? 1;
                var report = await _damageReportService.CreateReportAsync(currentUserId, dto);
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

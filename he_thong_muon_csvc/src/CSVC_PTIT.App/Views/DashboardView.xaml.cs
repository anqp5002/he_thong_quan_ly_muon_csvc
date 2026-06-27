using System.Windows;
using System.Windows.Controls;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        Loaded += DashboardView_Loaded;
    }

    private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            using var scope = App.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CsvcDbContext>();

            var totalAssets = await context.Assets.CountAsync();
            var totalRooms = await context.Rooms.CountAsync();
            var pendingRequests = await context.BorrowRequests
                .Where(r => r.Status == RequestStatus.Pending)
                .CountAsync();
            var totalUsers = await context.Users.Where(u => !u.IsDeleted).CountAsync();

            TxtTotalAssets.Text = totalAssets.ToString();
            TxtTotalRooms.Text = totalRooms.ToString();
            TxtPendingRequests.Text = pendingRequests.ToString();
            TxtTotalUsers.Text = totalUsers.ToString();
        }
        catch
        {
            // Giữ giá trị mặc định nếu lỗi
        }
    }
}

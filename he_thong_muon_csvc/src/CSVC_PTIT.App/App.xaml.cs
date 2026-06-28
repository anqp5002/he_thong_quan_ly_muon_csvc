using System.Windows;
using CSVC_PTIT.App.ViewModels;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Core.Services;
using CSVC_PTIT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSVC_PTIT.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Connection string đến SQL Server Docker.
    /// Server=localhost,14333  → Docker đã map port 14333 → 1433 bên trong
    /// Database=CsvcPtitDb    → Tên database (EF sẽ tự tạo nếu chưa có)
    /// User Id=sa             → Tài khoản admin mặc định của SQL Server
    /// Password=...           → Từ docker-compose.yml
    /// TrustServerCertificate → Bỏ qua SSL (vì chạy local)
    /// </summary>
    private const string ConnectionString =
        "Server=localhost,14333;Database=CsvcPtitDb;User Id=sa;Password=CsvcPtitCac@2026;TrustServerCertificate=True;Encrypt=False";

    /// <summary>
    /// DI Container — nơi đăng ký tất cả services và viewmodels.
    /// Bất kỳ đâu trong app đều có thể lấy service qua ServiceProvider.
    /// </summary>
    public static ServiceProvider ServiceProvider { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // === TẦNG DATA: Database ===
        services.AddDbContext<CsvcDbContext>(options =>
            options.UseSqlServer(ConnectionString));

        // === TẦNG CORE: Business Services ===
        services.AddSingleton<IAuthService, AuthService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IAssetService, AssetService>();
        services.AddTransient<IBorrowService, BorrowService>();
        services.AddTransient<ICheckoutService, CheckoutService>();
        services.AddTransient<IReturnService, ReturnService>();
        services.AddTransient<IDamageReportService, DamageReportService>();
        services.AddTransient<IReportService, ReportService>();
        services.AddTransient<IAuditLogService, AuditLogService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<INotificationService, NotificationService>();

        // === TẦNG APP: ViewModels ===
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ChangePasswordViewModel>();
        services.AddTransient<QuanLyTaiKhoanViewModel>();  // Sprint 1 — A.5
        services.AddTransient<DanhMucCSVCViewModel>();      // Sprint 1 — A.7
        services.AddTransient<CauHinhHeThongViewModel>();   // Sprint 1 — A.8
        services.AddTransient<NhatKyViewModel>();           // Sprint 1 — A.9
        services.AddTransient<TraCuuCSVCViewModel>();       // Sprint 1 — B.1
        services.AddTransient<ViewModels.SV.TheoDoiDonMuonViewModel>(); // Sprint 1 — B.4
        services.AddTransient<ViewModels.SV.DangKyMuonViewModel>();
        services.AddTransient<ViewModels.SV.YeuCauHoanTraViewModel>();

        // Mới bổ sung cho Sprint 2 & 1
        services.AddTransient<ViewModels.DT.DanhSachDonCanDuyetViewModel>();
        services.AddTransient<ViewModels.DT.TaoDonNgoaiGioViewModel>();
        services.AddTransient<ViewModels.QL.DanhSachDonDaDuyetViewModel>();
        services.AddTransient<ThongBaoViewModel>();

        ServiceProvider = services.BuildServiceProvider();

        // Seed dữ liệu mẫu (chỉ chạy lần đầu, lần sau tự bỏ qua)
        using var scope = ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CsvcDbContext>();
        await DatabaseSeeder.SeedAsync(db);
        await DatabaseSeederDevC.SeedDevCAsync(db); // Seed dữ liệu mẫu cho test chức năng Dev C

        // Prevent app from shutting down immediately when LoginView is closed
        Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // Mở cửa sổ đăng nhập trước
        var loginVm = ServiceProvider.GetRequiredService<LoginViewModel>();
        var loginView = new Views.LoginView(loginVm);
        
        if (loginView.ShowDialog() == true)
        {
            // Nếu login thành công, mở MainWindow
            var mainWindow = new MainWindow();
            Current.MainWindow = mainWindow;
            
            // Revert back to shutting down when the main window is closed
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            mainWindow.Show();
        }
        else
        {
            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Giải phóng tài nguyên khi app tắt
        ServiceProvider?.Dispose();
        base.OnExit(e);
    }
}

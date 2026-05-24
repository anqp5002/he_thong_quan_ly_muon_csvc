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
        "Server=localhost,14333;Database=CsvcPtitDb;User Id=sa;Password=CsvcPtitCac@2026;TrustServerCertificate=True";

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
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAssetService, AssetService>();
        
        // --- Dev C Services ---
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IReturnService, ReturnService>();
        services.AddScoped<IDamageReportService, DamageReportService>();

        // === TẦNG APP: ViewModels ===
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();

        ServiceProvider = services.BuildServiceProvider();

        // Seed dữ liệu mẫu (chỉ chạy lần đầu, lần sau tự bỏ qua)
        using var scope = ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CsvcDbContext>();
        await DatabaseSeeder.SeedAsync(db);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Giải phóng tài nguyên khi app tắt
        ServiceProvider?.Dispose();
        base.OnExit(e);
    }
}

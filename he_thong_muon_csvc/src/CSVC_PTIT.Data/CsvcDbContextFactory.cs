using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CSVC_PTIT.Data;

/// <summary>
/// Class này chỉ được sử dụng ở lúc lập trình (Design-Time) bởi công cụ "dotnet ef".
/// Công cụ Migration cần biết cách kết nối Database để sinh ra file SQL, nhưng nó không đọc được file App.xaml.cs của WPF.
/// Vì vậy ta cung cấp "Factory" này để mồi cho nó một Connection String tạm thời lúc chạy lệnh.
/// </summary>
public class CsvcDbContextFactory : IDesignTimeDbContextFactory<CsvcDbContext>
{
    public CsvcDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CsvcDbContext>();
        
        // Hardcode connection string giống hệt trong App.xaml.cs để chạy Migration
        optionsBuilder.UseSqlServer("Server=localhost,14333;Database=CsvcPtitDb;User Id=sa;Password=CsvcPtitCac@2026;TrustServerCertificate=True");

        return new CsvcDbContext(optionsBuilder.Options);
    }
}

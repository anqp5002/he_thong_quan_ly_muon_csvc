using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CSVC_PTIT.Data;

/// <summary>
/// Database context chính — đại diện cho toàn bộ cơ sở dữ liệu CsvcPtitDb.
/// Chứa 14 DbSet tương ứng 14 bảng trong SQL Server.
/// </summary>
public class CsvcDbContext : DbContext
{
    public CsvcDbContext(DbContextOptions<CsvcDbContext> options) : base(options) { }

    // ===== Nhóm 1: Users & Roles =====
    public DbSet<Role> Roles { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<User> Users { get; set; }

    // ===== Nhóm 2: Assets =====
    public DbSet<AssetCategory> AssetCategories { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Room> Rooms { get; set; }

    // ===== Nhóm 3: Borrow Requests =====
    public DbSet<BorrowRequest> BorrowRequests { get; set; }
    public DbSet<BorrowRequestAsset> BorrowRequestAssets { get; set; }
    public DbSet<BorrowRequestRoom> BorrowRequestRooms { get; set; }

    // ===== Nhóm 4: Checkout & Return =====
    public DbSet<Checkout> Checkouts { get; set; }
    public DbSet<CheckoutItem> CheckoutItems { get; set; }
    public DbSet<Return> Returns { get; set; }
    public DbSet<ReturnItem> ReturnItems { get; set; }

    // ===== Nhóm 5: Damage Reports =====
    public DbSet<DamageReport> DamageReports { get; set; }

    // ===== Nhóm 6: System & Logs =====
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<SystemConfig> SystemConfigs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Áp dụng tất cả Fluent API Configurations từ thư mục Configurations/
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CsvcDbContext).Assembly);
    }
}

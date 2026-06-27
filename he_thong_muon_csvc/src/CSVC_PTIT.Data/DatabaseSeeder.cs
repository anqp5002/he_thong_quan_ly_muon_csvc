using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;

namespace CSVC_PTIT.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(CsvcDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (!context.Roles.Any()) 
        {
            var roles = new List<Role> 
        {
            new Role { RoleCode = "SV", RoleName = "Sinh viên", Description = "Sinh viên mượn CSVC trong giờ học" },
            new Role { RoleCode = "DT", RoleName = "Đoàn thể", Description = "Bí thư Đoàn / CN CLB — tạo đơn mượn ngoài giờ" },
            new Role { RoleCode = "QL", RoleName = "Quản lý CSVC", Description = "Duyệt đơn, bàn giao, nhận trả" },
            new Role { RoleCode = "AD", RoleName = "Admin", Description = "Quản trị hệ thống toàn quyền" }
        };
        context.Roles.AddRange(roles);

        var departments = new List<Department> 
        {
            new Department { DepartmentCode = "CNTT", DepartmentName = "Khoa Công nghệ Thông tin" },
            new Department { DepartmentCode = "LCD-ATTT", DepartmentName = "LCĐ An toàn Thông tin" },
            new Department { DepartmentCode = "CLB-GT", DepartmentName = "CLB Guitar" }
        };
        context.Departments.AddRange(departments);

        var users = new List<User> 
        {
            new User 
            { 
                Username = "sv01", 
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("123456"), 
                Email = "n23dccn001@student.ptithcm.edu.vn", 
                FullName = "Nguyễn Văn An", 
                RoleId = 1, 
                BorrowerType = BorrowerType.Student, 
                IsEmailVerified = true 
            },
            new User 
            { 
                Username = "sv02", 
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("123456"), 
                Email = "n23dccn002@student.ptithcm.edu.vn", 
                FullName = "Trần Thị Bình", 
                RoleId = 1, 
                BorrowerType = BorrowerType.Student, 
                IsEmailVerified = true 
            },
            new User 
            { 
                Username = "gv01", 
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("123456"), 
                Email = "chaupm@ptithcm.edu.vn", 
                FullName = "Phạm Minh Châu", 
                RoleId = 1, 
                BorrowerType = BorrowerType.Lecturer, 
                IsEmailVerified = true 
            },
            new User 
            { 
                Username = "gv02", 
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("123456"), 
                Email = "dunglh@ptithcm.edu.vn", 
                FullName = "Lê Hoàng Dũng", 
                RoleId = 1, 
                BorrowerType = BorrowerType.Lecturer, 
                IsEmailVerified = true 
            },
            new User 
            { 
                Username = "dt01", 
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("123456"), 
                Email = "n22dccn010@student.ptithcm.edu.vn", 
                FullName = "Võ Thanh Em", 
                RoleId = 2, 
                BorrowerType = BorrowerType.Unit, 
                IsEmailVerified = true 
            },
            new User 
            { 
                Username = "dt02", 
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("123456"), 
                Email = "n22dccn011@student.ptithcm.edu.vn", 
                FullName = "Đặng Thùy Phương", 
                RoleId = 2, 
                BorrowerType = BorrowerType.Club, 
                IsEmailVerified = true 
            },
            new User 
            { 
                Username = "ql01", 
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("123456"), 
                Email = "anpham5002@gmail.com", 
                FullName = "Hoàng Quốc Gia", 
                RoleId = 3, 
                BorrowerType = BorrowerType.Staff, 
                IsEmailVerified = true 
            },
            new User 
            { 
                Username = "ql02", 
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("123456"), 
                Email = "habt@ptithcm.edu.vn", 
                FullName = "Bùi Thu Hà", 
                RoleId = 3, 
                BorrowerType = BorrowerType.Staff, 
                IsEmailVerified = true 
            },
            new User 
            { 
                Username = "admin", 
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("admin123"), 
                Email = "admin@ptithcm.edu.vn", 
                FullName = "Ngô Văn Khải", 
                RoleId = 4, 
                BorrowerType = BorrowerType.Admin, 
                IsEmailVerified = true 
            },
            new User 
            { 
                Username = "clb01", 
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("123456"), 
                Email = "n23dccn020@student.ptithcm.edu.vn", 
                FullName = "Lý Ngọc Linh", 
                RoleId = 2, 
                BorrowerType = BorrowerType.Club, 
                IsEmailVerified = true 
            }
        };
        context.Users.AddRange(users);

        var categories = new List<AssetCategory> 
        {
            new AssetCategory { CategoryCode = "TBDT", CategoryName = "Thiết bị điện tử" },
            new AssetCategory { CategoryCode = "BG", CategoryName = "Bàn ghế" },
            new AssetCategory { CategoryCode = "TB-AV", CategoryName = "Thiết bị âm thanh/ánh sáng" },
            new AssetCategory { CategoryCode = "DC", CategoryName = "Dụng cụ" },
            new AssetCategory { CategoryCode = "VPP", CategoryName = "Văn phòng phẩm" }
        };
        context.AssetCategories.AddRange(categories);

        var rooms = new List<Room> 
        {
            new Room { RoomCode = "B401", RoomName = "Phòng học B401", Building = "Tòa B", FloorNo = 4, Capacity = 50, RoomType = RoomType.Classroom },
            new Room { RoomCode = "B402", RoomName = "Phòng học B402", Building = "Tòa B", FloorNo = 4, Capacity = 50, RoomType = RoomType.Classroom },
            new Room { RoomCode = "HTD", RoomName = "Hội trường D", Building = "Tòa D", FloorNo = 1, Capacity = 300, RoomType = RoomType.Hall },
            new Room { RoomCode = "A301", RoomName = "Phòng thí nghiệm A301", Building = "Tòa A", FloorNo = 3, Capacity = 30, RoomType = RoomType.Lab },
            new Room { RoomCode = "A302", RoomName = "Phòng thí nghiệm A302", Building = "Tòa A", FloorNo = 3, Capacity = 30, RoomType = RoomType.Lab }
        };
        context.Rooms.AddRange(rooms);

        var assets = new List<Asset> 
        {
            new Asset 
            { 
                AssetCode = "MC-001", 
                AssetName = "Micro không dây Shure", 
                CategoryId = 1, 
                ManagementMode = ManagementMode.Item, 
                TotalQuantity = 1, 
                AvailableQuantity = 1 
            },
            new Asset 
            { 
                AssetCode = "MC-002", 
                AssetName = "Micro có dây Sony", 
                CategoryId = 1, 
                ManagementMode = ManagementMode.Item, 
                TotalQuantity = 1, 
                AvailableQuantity = 1 
            },
            new Asset 
            { 
                AssetCode = "PJ-001", 
                AssetName = "Máy chiếu Epson EB-X51",
                CategoryId = 1,
                ManagementMode = ManagementMode.Item,
                TotalQuantity = 1,
                AvailableQuantity = 1
            },
            new Asset 
            { 
                AssetCode = "PJ-002", 
                AssetName = "Máy chiếu BenQ MH733",
                CategoryId = 1,
                ManagementMode = ManagementMode.Item,
                TotalQuantity = 1,
                AvailableQuantity = 1
            },
            new Asset 
            { 
                AssetCode = "GN-SET", 
                AssetName = "Bộ ghế nhựa xanh",
                CategoryId = 2,
                ManagementMode = ManagementMode.Quantity,
                TotalQuantity = 100,
                AvailableQuantity = 100
            },
            new Asset 
            { 
                AssetCode = "BG-SET", 
                AssetName = "Bộ bàn gấp 1.8m",
                CategoryId = 2,
                ManagementMode = ManagementMode.Quantity,
                TotalQuantity = 30,
                AvailableQuantity = 30
            },
            new Asset 
            { 
                AssetCode = "LOA-001", 
                AssetName = "Loa kéo JBL PartyBox",
                CategoryId = 3,
                ManagementMode = ManagementMode.Item,
                TotalQuantity = 1,
                AvailableQuantity = 1
            },
            new Asset 
            { 
                AssetCode = "BB-SET", 
                AssetName = "Bảng trắng di động",
                CategoryId = 4,
                ManagementMode = ManagementMode.Quantity,
                TotalQuantity = 10,
                AvailableQuantity = 10
            },
            new Asset 
            { 
                AssetCode = "OD-SET", 
                AssetName = "Ổ cắm điện 6 lỗ",
                CategoryId = 4,
                ManagementMode = ManagementMode.Quantity,
                TotalQuantity = 20,
                AvailableQuantity = 20
            },
            new Asset 
            { 
                AssetCode = "VPP-SET", 
                AssetName = "Băng rôn/banner sự kiện",
                CategoryId = 5,
                ManagementMode = ManagementMode.Quantity,
                TotalQuantity = 5,
                AvailableQuantity = 5
            }
        };
            context.Assets.AddRange(assets);
        }

        if (!context.SystemConfigs.Any())
        {
            var configs = new List<SystemConfig> 
            {
                new SystemConfig { ConfigKey = "max_borrow_hours", ConfigValue = "8", Description = "Số giờ mượn tối đa cho sinh viên" },
                new SystemConfig { ConfigKey = "max_items_per_request", ConfigValue = "10", Description = "Số lượng thiết bị tối đa mỗi phiếu" },
                new SystemConfig { ConfigKey = "overdue_notify_minutes", ConfigValue = "30", Description = "Báo trước khi quá hạn (phút)" }
            };
            context.SystemConfigs.AddRange(configs);
        }

        await context.SaveChangesAsync();
    }
}
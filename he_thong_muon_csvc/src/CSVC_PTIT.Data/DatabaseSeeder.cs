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
        }

        if (!context.Rooms.Any(r => r.RoomCode == "2A08"))
        {
            var rooms = new List<Room>();

            // KHU A (Khu trung tâm)
            // Tầng trệt
            rooms.Add(new Room { RoomCode = "2A08", RoomName = "Phòng 2A08", Building = "Khu A", FloorNo = 1, RoomType = RoomType.Classroom });
            rooms.Add(new Room { RoomCode = "TC-HC", RoomName = "Phòng TC - HC (Tài chính - Hành chính)", Building = "Khu A", FloorNo = 1, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "Hội đồng", RoomName = "Phòng hội đồng", Building = "Khu A", FloorNo = 1, RoomType = RoomType.Hall });
            rooms.Add(new Room { RoomCode = "CSVC", RoomName = "Trung tâm CSVC & Dịch vụ", Building = "Khu A", FloorNo = 1, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "KTTC", RoomName = "Phòng KTTC (Kế toán Tài chính)", Building = "Khu A", FloorNo = 1, RoomType = RoomType.Office });
            
            // Lầu 1
            rooms.Add(new Room { RoomCode = "2A16", RoomName = "Phòng 2A16", Building = "Khu A", FloorNo = 2, RoomType = RoomType.Classroom });
            rooms.Add(new Room { RoomCode = "QTKD2", RoomName = "Khoa QTKD 2", Building = "Khu A", FloorNo = 2, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "LDHV", RoomName = "Lãnh đạo HV", Building = "Khu A", FloorNo = 2, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "VT2", RoomName = "Khoa Viễn Thông 2", Building = "Khu A", FloorNo = 2, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "CB2", RoomName = "Khoa Cơ Bản 2", Building = "Khu A", FloorNo = 2, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "DT-KHCN", RoomName = "Phòng Đào tạo & KHCN", Building = "Khu A", FloorNo = 2, RoomType = RoomType.Office });

            // Lầu 2
            for (int i = 23; i <= 27; i++)
                rooms.Add(new Room { RoomCode = $"2A{i}", RoomName = $"Giảng đường 2A{i}", Building = "Khu A", FloorNo = 3, RoomType = RoomType.Classroom });
            rooms.Add(new Room { RoomCode = "GV-A2", RoomName = "Phòng Giáo viên Lầu 2", Building = "Khu A", FloorNo = 3, RoomType = RoomType.Office });

            // Lầu 3
            for (int i = 32; i <= 36; i++)
                rooms.Add(new Room { RoomCode = $"2A{i}", RoomName = $"Giảng đường 2A{i}", Building = "Khu A", FloorNo = 4, RoomType = RoomType.Classroom });
            rooms.Add(new Room { RoomCode = "GV-A3", RoomName = "Phòng Giáo viên Lầu 3", Building = "Khu A", FloorNo = 4, RoomType = RoomType.Office });

            // KHU B (Dãy bên phải)
            // Tầng trệt
            rooms.Add(new Room { RoomCode = "KĐCLGD", RoomName = "Trung tâm khảo thí ĐBCL GD", Building = "Khu B", FloorNo = 1, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "CTSV", RoomName = "Phòng công tác sinh viên", Building = "Khu B", FloorNo = 1, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "GV-B", RoomName = "Phòng giáo vụ", Building = "Khu B", FloorNo = 1, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "2B03", RoomName = "Phòng hội thảo 2B03", Building = "Khu B", FloorNo = 1, RoomType = RoomType.Hall });

            // Lầu 1
            rooms.Add(new Room { RoomCode = "CNTT2", RoomName = "Khoa công nghệ thông tin 2", Building = "Khu B", FloorNo = 2, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "TH-B1", RoomName = "Phòng thực hành Lầu 1", Building = "Khu B", FloorNo = 2, RoomType = RoomType.Lab });

            // Lầu 2
            for (int i = 22; i <= 25; i++)
                rooms.Add(new Room { RoomCode = $"2B{i}", RoomName = $"Giảng đường 2B{i}", Building = "Khu B", FloorNo = 3, RoomType = RoomType.Classroom });
            rooms.Add(new Room { RoomCode = "TH-B2", RoomName = "Phòng thực hành Lầu 2", Building = "Khu B", FloorNo = 3, RoomType = RoomType.Lab });

            // Lầu 3
            for (int i = 32; i <= 34; i++)
                rooms.Add(new Room { RoomCode = $"2B{i}", RoomName = $"Giảng đường 2B{i}", Building = "Khu B", FloorNo = 4, RoomType = RoomType.Classroom });
            rooms.Add(new Room { RoomCode = "TH-B3", RoomName = "Phòng thực hành Lầu 3", Building = "Khu B", FloorNo = 4, RoomType = RoomType.Lab });

            // KHU C (Khối màu xanh lá)
            rooms.Add(new Room { RoomCode = "TV-T", RoomName = "Thư viện (Tầng trệt)", Building = "Khu C", FloorNo = 1, RoomType = RoomType.Hall });
            rooms.Add(new Room { RoomCode = "DT2", RoomName = "Khoa Điện tử 2", Building = "Khu C", FloorNo = 2, RoomType = RoomType.Office });
            rooms.Add(new Room { RoomCode = "TV-1", RoomName = "Thư viện (Tầng trên của thư viện)", Building = "Khu C", FloorNo = 2, RoomType = RoomType.Hall });

            // KHU D (Khối màu xanh dương)
            rooms.Add(new Room { RoomCode = "HT-D", RoomName = "HỘI TRƯỜNG D", Building = "Khu D", FloorNo = 1, RoomType = RoomType.Hall });
            rooms.Add(new Room { RoomCode = "TH-D1", RoomName = "Phòng thực hành 1", Building = "Khu D", FloorNo = 1, RoomType = RoomType.Lab });
            rooms.Add(new Room { RoomCode = "TH-D2", RoomName = "Phòng thực hành 2", Building = "Khu D", FloorNo = 1, RoomType = RoomType.Lab });

            // KHU E (Dãy bên trái)
            rooms.Add(new Room { RoomCode = "2E01", RoomName = "Phòng 2E01", Building = "Khu E", FloorNo = 1, RoomType = RoomType.Classroom });
            rooms.Add(new Room { RoomCode = "2E02", RoomName = "Phòng 2E02", Building = "Khu E", FloorNo = 1, RoomType = RoomType.Classroom });
            for (int i = 4; i <= 7; i++)
                rooms.Add(new Room { RoomCode = $"2E0{i}", RoomName = $"Giảng đường 2E0{i}", Building = "Khu E", FloorNo = 1, RoomType = RoomType.Classroom });

            rooms.Add(new Room { RoomCode = "GV-E1", RoomName = "Phòng giáo viên", Building = "Khu E", FloorNo = 2, RoomType = RoomType.Office });
            for (int i = 14; i <= 17; i++)
                rooms.Add(new Room { RoomCode = $"2E{i}", RoomName = $"Phòng học ngoại ngữ 2E{i}", Building = "Khu E", FloorNo = 2, RoomType = RoomType.Classroom });

            rooms.Add(new Room { RoomCode = "TN-VL", RoomName = "Phòng TN Vật lý", Building = "Khu E", FloorNo = 3, RoomType = RoomType.Lab });
            rooms.Add(new Room { RoomCode = "2E27", RoomName = "Giảng đường 2E27", Building = "Khu E", FloorNo = 3, RoomType = RoomType.Classroom });

            for (int i = 31; i <= 37; i++)
                rooms.Add(new Room { RoomCode = $"2E{i}", RoomName = $"Giảng đường 2E{i}", Building = "Khu E", FloorNo = 4, RoomType = RoomType.Classroom });

            var existingRoomCodes = context.Rooms.Select(r => r.RoomCode).ToHashSet();
            var roomsToAdd = rooms.Where(r => !existingRoomCodes.Contains(r.RoomCode)).ToList();
            
            if (roomsToAdd.Any())
            {
                context.Rooms.AddRange(roomsToAdd);
            }
        }

        if (!context.Assets.Any())
        {

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
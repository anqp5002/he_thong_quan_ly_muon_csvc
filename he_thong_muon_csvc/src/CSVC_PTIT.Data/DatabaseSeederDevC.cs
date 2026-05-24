using System;
using System.Linq;
using System.Threading.Tasks;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;

namespace CSVC_PTIT.Data;

public static class DatabaseSeederDevC
{
    public static async Task SeedDevCAsync(CsvcDbContext context)
    {
        if (context.BorrowRequests.Any()) 
            return;

        // Lấy người mượn mẫu (Sinh viên: id=1, Đoàn thể: id=5)
        var userSv = context.Users.FirstOrDefault(u => u.Username == "sv01");
        var userDt = context.Users.FirstOrDefault(u => u.Username == "dt01");
        var admin = context.Users.FirstOrDefault(u => u.Username == "admin");

        if (userSv == null || userDt == null || admin == null) return;

        // Lấy CSVC mẫu
        var mc001 = context.Assets.FirstOrDefault(a => a.AssetCode == "MC-001");
        var gnSet = context.Assets.FirstOrDefault(a => a.AssetCode == "GN-SET");
        
        if (mc001 == null || gnSet == null) return;

        // Tạo Đơn 1: Đã duyệt (Cần bàn giao)
        var req1 = new BorrowRequest
        {
            RequestCode = "SV-0001",
            RequestType = RequestType.InClass,
            ContactPhone = "0987654321",
            Title = "Mượn Micro học bù",
            Purpose = "Học bù môn CSDL",
            BorrowStartAt = DateTime.Now.AddHours(1),
            BorrowEndAt = DateTime.Now.AddHours(3),
            ExpectedReturnAt = DateTime.Now.AddHours(3),
            Status = RequestStatus.Approved,
            PriorityLevel = PriorityLevel.Normal,
            RequesterId = userSv.UserId,
            ApprovedBy = admin.UserId,
            ApprovedAt = DateTime.Now.AddHours(-1)
        };

        // Tạo Đơn 2: Đã duyệt (Cần bàn giao)
        var req2 = new BorrowRequest
        {
            RequestCode = "DT-0001",
            RequestType = RequestType.OffHours,
            ContactPhone = "0123456789",
            Title = "Tổ chức sự kiện IT",
            Purpose = "Sự kiện khoa CNTT",
            BorrowStartAt = DateTime.Now.AddDays(1),
            BorrowEndAt = DateTime.Now.AddDays(1).AddHours(4),
            ExpectedReturnAt = DateTime.Now.AddDays(1).AddHours(4),
            Status = RequestStatus.Approved,
            PriorityLevel = PriorityLevel.High,
            RequesterId = userDt.UserId,
            ApprovedBy = admin.UserId,
            ApprovedAt = DateTime.Now.AddHours(-2)
        };

        context.BorrowRequests.AddRange(req1, req2);
        await context.SaveChangesAsync();

        // Thêm Asset cho Đơn 1
        var req1Asset1 = new BorrowRequestAsset
        {
            QuantityRequested = 1,
            QuantityApproved = 1,
            QuantityCheckedOut = 0,
            QuantityReturned = 0,
            RequestId = req1.RequestId,
            AssetId = mc001.AssetId
        };

        // Thêm Asset cho Đơn 2
        var req2Asset1 = new BorrowRequestAsset
        {
            QuantityRequested = 50,
            QuantityApproved = 50,
            QuantityCheckedOut = 0,
            QuantityReturned = 0,
            RequestId = req2.RequestId,
            AssetId = gnSet.AssetId
        };

        context.BorrowRequestAssets.AddRange(req1Asset1, req2Asset1);
        await context.SaveChangesAsync();
    }
}

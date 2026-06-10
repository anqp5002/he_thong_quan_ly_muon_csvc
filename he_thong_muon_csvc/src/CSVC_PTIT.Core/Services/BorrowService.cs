using CSVC_PTIT.Core.DTOs;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CSVC_PTIT.Core.Services;

public class BorrowService : IBorrowService
{
    private readonly CsvcDbContext _context;

    public BorrowService(CsvcDbContext context)
    {
        _context = context;
    }

    public async Task<BorrowRequest> CreateInClassRequestAsync(CreateBorrowRequestDto dto)
    {
        // Sinh mã đơn tự động: SV-001, SV-002...
        var count = await _context.BorrowRequests.CountAsync() + 1;
        var requestCode = $"SV-{count:D3}";

        var request = new BorrowRequest
        {
            RequestCode = requestCode,
            RequestType = RequestType.InClass,
            RequesterId = dto.RequesterId,
            ContactPhone = dto.ContactPhone,
            Title = dto.Title,
            Purpose = dto.Purpose,
            RequestNote = dto.RequestNote,
            BorrowStartAt = dto.BorrowStartAt,
            BorrowEndAt = dto.BorrowEndAt,
            ExpectedReturnAt = dto.BorrowEndAt,
            Status = RequestStatus.Pending,
            PriorityLevel = PriorityLevel.Normal,
            CreatedAt = DateTime.Now
        };

        // Tạo các dòng CSVC trong đơn
        foreach (var item in dto.Assets)
        {
            request.BorrowRequestAssets.Add(new BorrowRequestAsset
            {
                AssetId = item.AssetId,
                QuantityRequested = item.QuantityRequested,
                QuantityApproved = 0,
                QuantityCheckedOut = 0,
                QuantityReturned = 0,
                ItemNote = item.ItemNote
            });
        }

        _context.BorrowRequests.Add(request);
        await _context.SaveChangesAsync();

        return request;
    }

    public async Task<List<BorrowRequest>> GetRequestsByUserAsync(int userId)
    {
        return await _context.BorrowRequests
            .Include(r => r.BorrowRequestAssets)
                .ThenInclude(a => a.Asset)
            .Where(r => r.RequesterId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}
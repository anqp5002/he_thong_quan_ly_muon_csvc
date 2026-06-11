using CSVC_PTIT.Core.DTOs;
using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

public interface IBorrowService
{
    Task<BorrowRequest> CreateInClassRequestAsync(CreateBorrowRequestDto dto);
    Task<List<BorrowRequest>> GetRequestsByUserAsync(int userId);
    Task<BorrowRequest> CreateOffHoursRequestAsync(CreateBorrowRequestDto dto);
}

using CSVC_PTIT.Core.DTOs;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;

namespace CSVC_PTIT.Core.Interfaces;

public interface IBorrowService
{
    Task<BorrowRequest> CreateInClassRequestAsync(CreateBorrowRequestDto dto);
    Task<List<BorrowRequest>> GetRequestsByUserAsync(int userId);
    Task<BorrowRequest> CreateOffHoursRequestAsync(CreateBorrowRequestDto dto);
    Task<List<BorrowRequest>> GetRequestsByStatusAsync(RequestStatus status);
    Task ApproveRequestAsync(int requestId, int approverId);
    Task RejectRequestAsync(int requestId, int approverId, string reason);
}

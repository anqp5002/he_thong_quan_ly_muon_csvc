using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Services;

/// <summary>
/// Service quản lý tài khoản — stub.
/// Sprint 1 — Task A.4 sẽ code logic thật.
/// </summary>
public class UserService : IUserService
{
    private readonly CsvcDbContext _context;

    public UserService(CsvcDbContext context)
    {
        _context = context;
    }

    public Task<List<User>> GetAllAsync()
        => throw new NotImplementedException("Sprint 1 — Task A.4");

    public Task<User?> GetByIdAsync(int userId)
        => throw new NotImplementedException("Sprint 1 — Task A.4");

    public Task<User> CreateAsync(User user, string password)
        => throw new NotImplementedException("Sprint 1 — Task A.4");

    public Task UpdateAsync(User user)
        => throw new NotImplementedException("Sprint 1 — Task A.4");

    public Task LockAsync(int userId)
        => throw new NotImplementedException("Sprint 1 — Task A.4");

    public Task UnlockAsync(int userId)
        => throw new NotImplementedException("Sprint 1 — Task A.4");
}

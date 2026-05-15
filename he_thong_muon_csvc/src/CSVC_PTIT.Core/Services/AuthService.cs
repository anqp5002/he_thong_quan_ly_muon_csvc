using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data;
using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Services;

/// <summary>
/// Service xác thực — stub (khung rỗng).
/// Sprint 1 — Task A.1 sẽ code logic thật.
/// </summary>
public class AuthService : IAuthService
{
    private readonly CsvcDbContext _context;

    public AuthService(CsvcDbContext context)
    {
        _context = context;
    }

    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public Task<User?> LoginAsync(string email, string password)
    {
        throw new NotImplementedException("Sprint 1 — Task A.1");
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}

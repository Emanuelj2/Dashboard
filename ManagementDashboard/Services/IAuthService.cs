using ManagementDashboard.Models;

namespace ManagementDashboard.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync (User user);
        Task<User?> ValidateUserAsync (string email, string password);
        Task<bool> Logout();
    }
}

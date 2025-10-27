using ManagementDashboard.Models;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace ManagementDashboard.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync(); //IEnumerable is read -only collection good for returning large datasets
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> CreateUserAsync(User user);
        Task<User?> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);

    }
}

using ManagementDashboard.Data;
using ManagementDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementDashboard.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new user
        public async Task<User?> CreateUserAsync(User user)
        {
            // Check for existing email asynchronously
            var exist = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (exist != null)
            {
                return null; // User with this email already exists
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Delete a user by ID
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false; // User not found
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Get a user by ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id); // returns null if not found
        }

        // Update a user
        public async Task<User?> UpdateUserAsync(int id, User updatedUser)
        {
            var exist = await _context.Users.FindAsync(id);
            if (exist == null)
            {
                return null; // User not found
            }

            // Update properties
            exist.Name = updatedUser.Name;
            exist.Email = updatedUser.Email;
            exist.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
            return exist;
        }
    }
}

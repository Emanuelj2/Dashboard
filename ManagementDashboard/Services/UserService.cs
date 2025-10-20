using ManagementDashboard.Models;

namespace ManagementDashboard.Services
{
    public class UserService : IUserService
    {
        private readonly IUserService _userService;

        public UserService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<User> AddUserAsync(User user)
        {
            var existingUser = _userService.GetUserByIdAsync(user.Id).Result;
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with the same ID already exists.");
            }

            //create user
            return await _userService.AddUserAsync(user);

        }

        public Task<User> DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetUserByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}

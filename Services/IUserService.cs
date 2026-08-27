using CosmosCrudApi.Models;

namespace CosmosCrudApi.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);

        Task<List<User>> GetUsersAsync();

        Task<User?> GetUserByIdAsync(string id);

        Task<User?> UpdateUserAsync(string id, User user);

        Task<bool> DeleteUserAsync(string id);
    }
}
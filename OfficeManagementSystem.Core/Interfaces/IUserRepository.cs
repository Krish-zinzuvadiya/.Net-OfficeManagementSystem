using System.Collections.Generic;
using System.Threading.Tasks;
using OfficeManagementSystem.Core.Entities;

namespace OfficeManagementSystem.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
}

using System.Collections.Generic;
using System.Threading.Tasks;
using OfficeManagementSystem.Core.Entities;

namespace OfficeManagementSystem.Core.Interfaces;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(int id);
    Task<IEnumerable<Department>> GetAllAsync();
    Task<Department> AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(int id);
}

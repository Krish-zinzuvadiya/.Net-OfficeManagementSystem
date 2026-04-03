using System.Collections.Generic;
using System.Threading.Tasks;
using OfficeManagementSystem.Core.Entities;

namespace OfficeManagementSystem.Core.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee> AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int id);
}

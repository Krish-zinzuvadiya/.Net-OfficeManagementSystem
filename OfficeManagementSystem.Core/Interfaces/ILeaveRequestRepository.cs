using System.Collections.Generic;
using System.Threading.Tasks;
using OfficeManagementSystem.Core.Entities;

namespace OfficeManagementSystem.Core.Interfaces;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> GetByIdAsync(int id);
    Task<IEnumerable<LeaveRequest>> GetAllAsync();
    Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
    Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest);
    Task UpdateAsync(LeaveRequest leaveRequest);
    Task DeleteAsync(int id);
}

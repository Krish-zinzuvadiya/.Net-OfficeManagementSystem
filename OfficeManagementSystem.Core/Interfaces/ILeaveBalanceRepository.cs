using System.Collections.Generic;
using System.Threading.Tasks;
using OfficeManagementSystem.Core.Entities;

namespace OfficeManagementSystem.Core.Interfaces;

public interface ILeaveBalanceRepository
{
    Task<IEnumerable<LeaveBalance>> GetByEmployeeIdAsync(int employeeId, int year);
    Task<LeaveBalance?> GetByEmployeeAndTypeAsync(int employeeId, LeaveType leaveType, int year);
    Task<LeaveBalance> AddAsync(LeaveBalance leaveBalance);
    Task UpdateAsync(LeaveBalance leaveBalance);
    Task InitializeBalancesForEmployee(int employeeId, int year);
}

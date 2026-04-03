using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OfficeManagementSystem.Core.Entities;
using OfficeManagementSystem.Core.Interfaces;

namespace OfficeManagementSystem.Infrastructure.Data.Repositories;

public class LeaveBalanceRepository : ILeaveBalanceRepository
{
    private readonly AppDbContext _context;

    public LeaveBalanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LeaveBalance>> GetByEmployeeIdAsync(int employeeId, int year)
    {
        return await _context.LeaveBalances
            .Where(lb => lb.EmployeeId == employeeId && lb.Year == year)
            .ToListAsync();
    }

    public async Task<LeaveBalance?> GetByEmployeeAndTypeAsync(int employeeId, LeaveType leaveType, int year)
    {
        return await _context.LeaveBalances
            .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId && lb.LeaveType == leaveType && lb.Year == year);
    }

    public async Task<LeaveBalance> AddAsync(LeaveBalance leaveBalance)
    {
        _context.LeaveBalances.Add(leaveBalance);
        await _context.SaveChangesAsync();
        return leaveBalance;
    }

    public async Task UpdateAsync(LeaveBalance leaveBalance)
    {
        _context.LeaveBalances.Update(leaveBalance);
        await _context.SaveChangesAsync();
    }

    public async Task InitializeBalancesForEmployee(int employeeId, int year)
    {
        var existing = await _context.LeaveBalances
            .AnyAsync(lb => lb.EmployeeId == employeeId && lb.Year == year);

        if (!existing)
        {
            var balances = new[]
            {
                new LeaveBalance { EmployeeId = employeeId, LeaveType = LeaveType.CasualLeave, Total = 6, Used = 0, Year = year },
                new LeaveBalance { EmployeeId = employeeId, LeaveType = LeaveType.SickLeave, Total = 6, Used = 0, Year = year },
                new LeaveBalance { EmployeeId = employeeId, LeaveType = LeaveType.EarnedLeave, Total = 12, Used = 0, Year = year },
                new LeaveBalance { EmployeeId = employeeId, LeaveType = LeaveType.LeaveWithoutPay, Total = 48, Used = 0, Year = year },
            };
            _context.LeaveBalances.AddRange(balances);
            await _context.SaveChangesAsync();
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.Core.Entities;
using OfficeManagementSystem.Core.Interfaces;
using System.Security.Claims;
using OfficeManagementSystem.API.DTOs;

namespace OfficeManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILeaveBalanceRepository _leaveBalanceRepository;

    public LeaveRequestsController(
        ILeaveRequestRepository leaveRequestRepository, 
        IUserRepository userRepository,
        ILeaveBalanceRepository leaveBalanceRepository)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _userRepository = userRepository;
        _leaveBalanceRepository = leaveBalanceRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> GetAll()
    {
        var requests = await _leaveRequestRepository.GetAllAsync();
        return Ok(requests);
    }

    [HttpGet("my-requests")]
    public async Task<IActionResult> GetMyRequests()
    {
        var userIdString = User.FindFirstValue("UserId");
        if (!int.TryParse(userIdString, out var userId)) return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.Employee == null) return BadRequest("Employee not found for current user.");

        var requests = await _leaveRequestRepository.GetByEmployeeIdAsync(user.Employee.Id);
        return Ok(requests);
    }

    [HttpGet("my-balances")]
    public async Task<IActionResult> GetMyBalances()
    {
        var userIdString = User.FindFirstValue("UserId");
        if (!int.TryParse(userIdString, out var userId)) return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.Employee == null) return BadRequest("Employee not found for current user.");

        var year = DateTime.Now.Year;
        
        // Initialize balances if they don't exist for this year
        await _leaveBalanceRepository.InitializeBalancesForEmployee(user.Employee.Id, year);
        
        var balances = await _leaveBalanceRepository.GetByEmployeeIdAsync(user.Employee.Id, year);
        
        return Ok(balances.Select(b => new {
            leaveType = b.LeaveType,
            leaveTypeName = b.LeaveType switch
            {
                LeaveType.CasualLeave => "Casual Leave",
                LeaveType.SickLeave => "Sick Leave",
                LeaveType.EarnedLeave => "Earned Leave",
                LeaveType.LeaveWithoutPay => "LWP (Leave Without Pay)",
                _ => "Unknown"
            },
            total = b.Total,
            used = b.Used,
            balance = b.Balance,
            year = b.Year
        }));
    }

    [HttpGet("my-info")]
    public async Task<IActionResult> GetMyInfo()
    {
        var userIdString = User.FindFirstValue("UserId");
        if (!int.TryParse(userIdString, out var userId)) return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.Employee == null) return BadRequest("Employee not found.");

        return Ok(new { 
            firstName = user.Employee.FirstName, 
            lastName = user.Employee.LastName,
            role = user.Role.ToString()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LeaveRequestCreateDto dto)
    {
        var userIdString = User.FindFirstValue("UserId");
        if (!int.TryParse(userIdString, out var userId)) return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.Employee == null) return BadRequest("Employee not found for current user.");

        var year = DateTime.Now.Year;

        // Initialize balances if needed
        await _leaveBalanceRepository.InitializeBalancesForEmployee(user.Employee.Id, year);

        // Check balance
        var balance = await _leaveBalanceRepository.GetByEmployeeAndTypeAsync(user.Employee.Id, dto.LeaveType, year);
        if (balance == null)
        {
            return BadRequest(new { message = "Leave balance not found." });
        }

        // Calculate days
        double daysRequested;
        if (dto.DayType == DayType.HalfDay)
        {
            daysRequested = 0.5;
        }
        else
        {
            daysRequested = (dto.EndDate.Date - dto.StartDate.Date).Days + 1;
            if (daysRequested <= 0) daysRequested = 1;
        }

        if (balance.Balance < daysRequested)
        {
            var typeName = dto.LeaveType switch
            {
                LeaveType.CasualLeave => "Casual Leave",
                LeaveType.SickLeave => "Sick Leave",
                LeaveType.EarnedLeave => "Earned Leave",
                LeaveType.LeaveWithoutPay => "Leave Without Pay",
                _ => "Leave"
            };
            return BadRequest(new { message = $"Insufficient {typeName} balance. Available: {balance.Balance}, Requested: {daysRequested}" });
        }

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = user.Employee.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveStatus.Pending,
            LeaveType = dto.LeaveType,
            DayType = dto.DayType,
            HalfDayPeriod = dto.HalfDayPeriod
        };
        
        var created = await _leaveRequestRepository.AddAsync(leaveRequest);
        return Ok(created);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] LeaveStatus status)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
        if (leaveRequest == null) return NotFound();

        // HR leaves can only be approved by Admin
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
        if (leaveRequest.Employee?.User?.Role == Role.HR && currentUserRole != "Admin")
        {
            return BadRequest(new { message = "HR leaves can only be approved/rejected by Admin." });
        }

        var previousStatus = leaveRequest.Status;
        leaveRequest.Status = status;

        // If approving, deduct from balance
        if (status == LeaveStatus.Approved && previousStatus != LeaveStatus.Approved)
        {
            var year = leaveRequest.StartDate.Year;
            var balance = await _leaveBalanceRepository.GetByEmployeeAndTypeAsync(
                leaveRequest.EmployeeId, leaveRequest.LeaveType, year);

            if (balance != null)
            {
                double days;
                if (leaveRequest.DayType == DayType.HalfDay)
                {
                    days = 0.5;
                }
                else
                {
                    days = (leaveRequest.EndDate.Date - leaveRequest.StartDate.Date).Days + 1;
                    if (days <= 0) days = 1;
                }

                if (balance.Balance < days)
                {
                    return BadRequest(new { message = "Employee has insufficient leave balance for this approval." });
                }

                balance.Used += days;
                await _leaveBalanceRepository.UpdateAsync(balance);
            }
        }
        // If un-approving (rejecting a previously approved), restore balance
        else if (status == LeaveStatus.Rejected && previousStatus == LeaveStatus.Approved)
        {
            var year = leaveRequest.StartDate.Year;
            var balance = await _leaveBalanceRepository.GetByEmployeeAndTypeAsync(
                leaveRequest.EmployeeId, leaveRequest.LeaveType, year);

            if (balance != null)
            {
                double days;
                if (leaveRequest.DayType == DayType.HalfDay)
                {
                    days = 0.5;
                }
                else
                {
                    days = (leaveRequest.EndDate.Date - leaveRequest.StartDate.Date).Days + 1;
                    if (days <= 0) days = 1;
                }

                balance.Used = Math.Max(0, balance.Used - days);
                await _leaveBalanceRepository.UpdateAsync(balance);
            }
        }

        await _leaveRequestRepository.UpdateAsync(leaveRequest);
        
        return NoContent();
    }
}

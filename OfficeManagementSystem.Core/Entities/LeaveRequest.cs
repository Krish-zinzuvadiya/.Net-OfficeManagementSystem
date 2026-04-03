using System;

namespace OfficeManagementSystem.Core.Entities;

public enum LeaveStatus
{
    Pending,
    Approved,
    Rejected
}

public enum LeaveType
{
    CasualLeave,
    SickLeave,
    EarnedLeave,
    LeaveWithoutPay
}

public enum DayType
{
    FullDay,
    HalfDay
}

public enum HalfDayPeriod
{
    None,
    FirstHalf,
    SecondHalf
}

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public LeaveType LeaveType { get; set; } = LeaveType.CasualLeave;
    public DayType DayType { get; set; } = DayType.FullDay;
    public HalfDayPeriod HalfDayPeriod { get; set; } = HalfDayPeriod.None;
}

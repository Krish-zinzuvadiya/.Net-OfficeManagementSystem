using System;
using OfficeManagementSystem.Core.Entities;

namespace OfficeManagementSystem.API.DTOs;

public class LeaveRequestCreateDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveType LeaveType { get; set; } = LeaveType.CasualLeave;
    public DayType DayType { get; set; } = DayType.FullDay;
    public HalfDayPeriod HalfDayPeriod { get; set; } = HalfDayPeriod.None;
}

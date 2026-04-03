using System;
using System.Collections.Generic;

namespace OfficeManagementSystem.Core.Entities;

public enum Designation
{
    Developer,
    Manager,
    GraphicDesigner,
    VideoEditor
}

public class Employee
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public Designation Designation { get; set; } = Designation.Developer;

    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
}

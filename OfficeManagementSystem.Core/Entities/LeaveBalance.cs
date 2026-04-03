namespace OfficeManagementSystem.Core.Entities;

public class LeaveBalance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public LeaveType LeaveType { get; set; }
    public double Total { get; set; }
    public double Used { get; set; }
    public int Year { get; set; }

    public double Balance => Total - Used;
}

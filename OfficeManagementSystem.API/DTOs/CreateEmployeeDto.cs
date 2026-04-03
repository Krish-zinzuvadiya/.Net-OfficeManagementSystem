using OfficeManagementSystem.Core.Entities;

namespace OfficeManagementSystem.API.DTOs;

public class CreateEmployeeDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int? DepartmentId { get; set; }
    public Role Role { get; set; } = Role.Employee;
    public Designation Designation { get; set; } = Designation.Developer;
}

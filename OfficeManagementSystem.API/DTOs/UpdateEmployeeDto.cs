using OfficeManagementSystem.Core.Entities;

namespace OfficeManagementSystem.API.DTOs;

public class UpdateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int? DepartmentId { get; set; }
    public Designation Designation { get; set; } = Designation.Developer;
}

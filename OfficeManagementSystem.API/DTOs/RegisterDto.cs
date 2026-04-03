using OfficeManagementSystem.Core.Entities;

namespace OfficeManagementSystem.API.DTOs;

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Role Role { get; set; }
}

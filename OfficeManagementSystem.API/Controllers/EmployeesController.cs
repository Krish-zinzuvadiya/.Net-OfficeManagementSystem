using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.API.DTOs;
using OfficeManagementSystem.Core.Entities;
using OfficeManagementSystem.Core.Interfaces;

namespace OfficeManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRepository _userRepository;

    public EmployeesController(IEmployeeRepository employeeRepository, IUserRepository userRepository)
    {
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotFound();

        return Ok(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Username is already taken." });
        }

        // Set DepartmentId to null if it's 0 or not provided to avoid FK constraint errors
        int? deptId = (dto.DepartmentId.HasValue && dto.DepartmentId.Value > 0) ? dto.DepartmentId : null;

        var newUser = new User
        {
            Username = dto.Username,
            PasswordHash = dto.Password, 
            Role = dto.Role,
            Employee = new Employee 
            { 
                FirstName = dto.FirstName, 
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DepartmentId = deptId,
                Designation = dto.Designation
            } 
        };

        try
        {
            var createdUser = await _userRepository.AddAsync(newUser);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Employee!.Id }, createdUser.Employee);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Failed to create employee.", detail = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var existingEmployee = await _employeeRepository.GetByIdAsync(id);
        if (existingEmployee == null) return NotFound();

        existingEmployee.FirstName = dto.FirstName;
        existingEmployee.LastName = dto.LastName;
        existingEmployee.Email = dto.Email;
        existingEmployee.Phone = dto.Phone;
        existingEmployee.DepartmentId = (dto.DepartmentId.HasValue && dto.DepartmentId.Value > 0) ? dto.DepartmentId : null;
        existingEmployee.Designation = dto.Designation;

        try
        {
            await _employeeRepository.UpdateAsync(existingEmployee);
            return NoContent();
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Failed to update employee.", detail = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotFound();
        
        await _userRepository.DeleteAsync(employee.UserId);
        return NoContent();
    }
}

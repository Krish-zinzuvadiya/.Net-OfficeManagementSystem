using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeManagementSystem.Core.Interfaces;
using OfficeManagementSystem.Infrastructure.Data;
using OfficeManagementSystem.Infrastructure.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Setup Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Data Source=OfficeManagement.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Dependency Injection for Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<ILeaveBalanceRepository, LeaveBalanceRepository>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is missing.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure Database is Created & Initialized
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    // Seed default users if none exist
    if (!context.Users.Any())
    {
        var adminUser = new OfficeManagementSystem.Core.Entities.User
        {
            Username = "admin",
            PasswordHash = "password123",
            Role = OfficeManagementSystem.Core.Entities.Role.Admin,
            Employee = new OfficeManagementSystem.Core.Entities.Employee 
            { 
                FirstName = "Admin", 
                LastName = "User", 
                Email = "admin@office.com" 
            }
        };
        var hrUser = new OfficeManagementSystem.Core.Entities.User
        {
            Username = "hr",
            PasswordHash = "password123",
            Role = OfficeManagementSystem.Core.Entities.Role.HR,
            Employee = new OfficeManagementSystem.Core.Entities.Employee 
            { 
                FirstName = "HR", 
                LastName = "Manager", 
                Email = "hr@office.com" 
            }
        };
        var employeeUser = new OfficeManagementSystem.Core.Entities.User
        {
            Username = "employee",
            PasswordHash = "password123",
            Role = OfficeManagementSystem.Core.Entities.Role.Employee,
            Employee = new OfficeManagementSystem.Core.Entities.Employee 
            { 
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john@office.com" 
            }
        };
        context.Users.AddRange(adminUser, hrUser, employeeUser);
        context.SaveChanges();

        // Seed leave balances for all employees
        var currentYear = DateTime.Now.Year;
        foreach (var emp in context.Employees.ToList())
        {
            if (!context.LeaveBalances.Any(lb => lb.EmployeeId == emp.Id && lb.Year == currentYear))
            {
                context.LeaveBalances.AddRange(
                    new OfficeManagementSystem.Core.Entities.LeaveBalance { EmployeeId = emp.Id, LeaveType = OfficeManagementSystem.Core.Entities.LeaveType.CasualLeave, Total = 6, Used = 0, Year = currentYear },
                    new OfficeManagementSystem.Core.Entities.LeaveBalance { EmployeeId = emp.Id, LeaveType = OfficeManagementSystem.Core.Entities.LeaveType.SickLeave, Total = 6, Used = 0, Year = currentYear },
                    new OfficeManagementSystem.Core.Entities.LeaveBalance { EmployeeId = emp.Id, LeaveType = OfficeManagementSystem.Core.Entities.LeaveType.EarnedLeave, Total = 12, Used = 0, Year = currentYear },
                    new OfficeManagementSystem.Core.Entities.LeaveBalance { EmployeeId = emp.Id, LeaveType = OfficeManagementSystem.Core.Entities.LeaveType.LeaveWithoutPay, Total = 48, Used = 0, Year = currentYear }
                );
            }
        }
        context.SaveChanges();
    }
}

app.Run();

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PCFirmApp.Models;

namespace PCFirmApp.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<Customer?> RegisterAsync(string username, string email, string password, string? address = null, string? phoneNumber = null)
    {
        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == username))
            return null;

        if (await _context.Users.AnyAsync(u => u.Email == email))
            return null;

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = hashedPassword,
            Role = UserRole.Customer,
            Address = address,
            PhoneNumber = phoneNumber,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Employee?> AddEmployeeAsync(string username, string email, string password, UserRole role)
    {
        if (role != UserRole.SeniorEmployee && role != UserRole.JuniorEmployee)
            return null;

        if (await _context.Users.AnyAsync(u => u.Username == username))
            return null;

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = hashedPassword,
            Role = role,
            EmploymentDate = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }
}

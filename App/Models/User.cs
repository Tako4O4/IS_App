using System;
using System.Collections.Generic;

namespace PCFirmApp.Models;

public abstract class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required UserRole Role { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public class Manager : User
{
}

public class Employee : User
{
    public DateTime? EmploymentDate { get; set; }
}

public class Customer : User
{
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<ServiceRequest> ServiceRequests { get; set; } = [];
    public ICollection<ProductReview> Reviews { get; set; } = [];
}

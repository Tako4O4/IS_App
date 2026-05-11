using System;

namespace PCFirmApp.Models;

public class ServiceRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public ServiceStatus Status { get; set; } = ServiceStatus.Pending;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public required string Description { get; set; }
    public required string ContactPhone { get; set; }
    public required string ContactEmail { get; set; }
    public DateTime DropoffDate { get; set; }
    public DateTime? EstimatedCompletionDate { get; set; }
    public string? Notes { get; set; }

    public Customer? Customer { get; set; }
    public User? AssignedToUser { get; set; }
}

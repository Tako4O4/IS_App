using System;

namespace PCFirmApp.Models;

public class ProductReview
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public Guid CustomerId { get; set; }
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public Product? Product { get; set; }
    public Customer? Customer { get; set; }
}

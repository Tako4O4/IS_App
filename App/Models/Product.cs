using System;
using System.Collections.Generic;

namespace PCFirmApp.Models;

public abstract class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public required ProductCategory Category { get; set; }
    public int StockCount { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public Guid? CreatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
    public double AverageRating { get; set; } = 0;

    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<ProductReview> Reviews { get; set; } = [];
    public ICollection<PromotionProduct> PromotionProducts { get; set; } = [];
}

public class PreAssembledPC : Product
{
    public string? Specifications { get; set; }
}

public class Component : Product
{
    public required ComponentType ComponentType { get; set; }
    public string? CompatibilityNotes { get; set; }
}

using System;
using System.Collections.Generic;

namespace PCFirmApp.Models;

public class Promotion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public decimal DiscountPercent { get; set; } = 10m;
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public User? CreatedByUser { get; set; }
    public ICollection<PromotionProduct> PromotionProducts { get; set; } = [];
}

public class PromotionProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PromotionId { get; set; }
    public Guid ProductId { get; set; }

    public Promotion? Promotion { get; set; }
    public Product? Product { get; set; }
}

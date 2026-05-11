using System;
using System.Collections.Generic;

namespace PCFirmApp.Models;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public Guid? FulfilledByUserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveryDate { get; set; }
    public decimal TotalPrice { get; set; }

    public Customer? Customer { get; set; }
    public User? FulfilledByUser { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
}

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsPromotionDiscount { get; set; }
    public string? PromotionLabel { get; set; }

    public Order? Order { get; set; }
    public Product? Product { get; set; }
}

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using PCFirmApp.Models;

namespace PCFirmApp.Services;

public class AppDbContext : DbContext
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<Manager> Managers { get; set; }
    public required DbSet<Employee> Employees { get; set; }
    public required DbSet<Customer> Customers { get; set; }
    public required DbSet<Product> Products { get; set; }
    public required DbSet<PreAssembledPC> PreAssembledPCs { get; set; }
    public required DbSet<Component> Components { get; set; }
    public required DbSet<Order> Orders { get; set; }
    public required DbSet<OrderItem> OrderItems { get; set; }
    public required DbSet<ServiceRequest> ServiceRequests { get; set; }
    public required DbSet<Promotion> Promotions { get; set; }
    public required DbSet<PromotionProduct> PromotionProducts { get; set; }
    public required DbSet<ProductReview> ProductReviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pcfirm.db");
        options.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TPH for User hierarchy
        modelBuilder.Entity<User>().HasDiscriminator<string>("UserType")
            .HasValue<Manager>("Manager")
            .HasValue<Employee>("Employee")
            .HasValue<Customer>("Customer");

        // TPH for Product hierarchy
        modelBuilder.Entity<Product>().HasDiscriminator<string>("ProductType")
            .HasValue<PreAssembledPC>("PreAssembledPC")
            .HasValue<Component>("Component");

        // Relationships
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.ServiceRequests)
            .WithOne(sr => sr.Customer)
            .HasForeignKey(sr => sr.CustomerId);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Reviews)
            .WithOne(pr => pr.Customer)
            .HasForeignKey(pr => pr.CustomerId);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Reviews)
            .WithOne(pr => pr.Product)
            .HasForeignKey(pr => pr.ProductId);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.PromotionProducts)
            .WithOne(pp => pp.Product)
            .HasForeignKey(pp => pp.ProductId);

        modelBuilder.Entity<Promotion>()
            .HasMany(p => p.PromotionProducts)
            .WithOne(pp => pp.Promotion)
            .HasForeignKey(pp => pp.PromotionId);

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var managerId = Guid.NewGuid();
        var seniorEmpId = Guid.NewGuid();
        var juniorEmpId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        // Manager account
        var managerHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        modelBuilder.Entity<Manager>().HasData(new Manager
        {
            Id = managerId,
            Username = "manager",
            Email = "manager@pcfirm.com",
            PasswordHash = managerHash,
            Role = UserRole.Manager,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        });

        // Sample employees
        var seniorHash = BCrypt.Net.BCrypt.HashPassword("senior123");
        modelBuilder.Entity<Employee>().HasData(new Employee
        {
            Id = seniorEmpId,
            Username = "senior_emp",
            Email = "senior@pcfirm.com",
            PasswordHash = seniorHash,
            Role = UserRole.SeniorEmployee,
            EmploymentDate = DateTime.UtcNow.AddMonths(-6),
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        });

        var juniorHash = BCrypt.Net.BCrypt.HashPassword("junior123");
        modelBuilder.Entity<Employee>().HasData(new Employee
        {
            Id = juniorEmpId,
            Username = "junior_emp",
            Email = "junior@pcfirm.com",
            PasswordHash = juniorHash,
            Role = UserRole.JuniorEmployee,
            EmploymentDate = DateTime.UtcNow.AddMonths(-2),
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        });

        // Sample customer
        var customerHash = BCrypt.Net.BCrypt.HashPassword("customer123");
        modelBuilder.Entity<Customer>().HasData(new Customer
        {
            Id = customerId,
            Username = "customer1",
            Email = "customer@example.com",
            PasswordHash = customerHash,
            Role = UserRole.Customer,
            Address = "123 Main St, Anytown",
            PhoneNumber = "555-1234",
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        });

        // Sample products
        var desktopPcId = Guid.NewGuid();
        var laptopPcId = Guid.NewGuid();
        var cpuId = Guid.NewGuid();
        var gpuId = Guid.NewGuid();
        var ramId = Guid.NewGuid();
        var ssdId = Guid.NewGuid();
        var printerId = Guid.NewGuid();
        var mouseId = Guid.NewGuid();

        modelBuilder.Entity<PreAssembledPC>().HasData(
            new PreAssembledPC
            {
                Id = desktopPcId,
                Name = "Gaming Desktop Pro",
                Description = "High-performance gaming desktop with RTX 4080, Intel i9-13900K, 32GB RAM, 1TB NVMe SSD. Perfect for gaming and content creation.",
                Price = 2499.99m,
                Category = ProductCategory.DesktopPC,
                StockCount = 5,
                CreatedByUserId = seniorEmpId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                AverageRating = 4.5,
                Specifications = "Intel i9-13900K, RTX 4080, 32GB DDR5, 1TB NVMe"
            },
            new PreAssembledPC
            {
                Id = laptopPcId,
                Name = "Ultrabook Pro 15",
                Description = "Lightweight ultrabook with Intel i7, 16GB RAM, 512GB SSD. Ideal for professionals on the go.",
                Price = 1299.99m,
                Category = ProductCategory.LaptopPC,
                StockCount = 3,
                CreatedByUserId = seniorEmpId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                AverageRating = 4.8,
                Specifications = "Intel i7-1360P, 16GB DDR5, 512GB SSD"
            }
        );

        modelBuilder.Entity<Component>().HasData(
            new Component
            {
                Id = cpuId,
                Name = "Intel Core i9-13900K",
                Description = "High-end processor with 24 cores. Excellent for gaming and heavy workloads.",
                Price = 589.99m,
                Category = ProductCategory.DesktopPC,
                ComponentType = ComponentType.CPU,
                StockCount = 10,
                CreatedByUserId = seniorEmpId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                AverageRating = 4.9
            },
            new Component
            {
                Id = gpuId,
                Name = "NVIDIA RTX 4080 Super",
                Description = "Premium graphics card for 4K gaming and professional work. 16GB GDDR6X memory.",
                Price = 1199.99m,
                Category = ProductCategory.DesktopPC,
                ComponentType = ComponentType.GPU,
                StockCount = 7,
                CreatedByUserId = seniorEmpId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                AverageRating = 4.7
            },
            new Component
            {
                Id = ramId,
                Name = "Corsair Vengeance 32GB DDR5",
                Description = "32GB (2x16GB) DDR5 memory kit running at 6000MHz. Perfect for high-performance builds.",
                Price = 199.99m,
                Category = ProductCategory.DesktopPC,
                ComponentType = ComponentType.RAM,
                StockCount = 20,
                CreatedByUserId = seniorEmpId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                AverageRating = 4.6
            },
            new Component
            {
                Id = ssdId,
                Name = "Samsung 990 Pro 1TB NVMe SSD",
                Description = "Ultra-fast NVMe SSD with read speeds up to 7,450 MB/s. Great for gaming and content creation.",
                Price = 119.99m,
                Category = ProductCategory.DesktopPC,
                ComponentType = ComponentType.SSD,
                StockCount = 15,
                CreatedByUserId = seniorEmpId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                AverageRating = 4.8
            }
        );

        modelBuilder.Entity<PreAssembledPC>().HasData(
            new PreAssembledPC
            {
                Id = printerId,
                Name = "HP LaserJet Pro MFP",
                Description = "Professional multifunction printer for offices. Print, scan, copy, and fax in one device.",
                Price = 349.99m,
                Category = ProductCategory.Printer,
                StockCount = 4,
                CreatedByUserId = seniorEmpId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                AverageRating = 4.3,
                Specifications = "Print, Scan, Copy, Fax"
            }
        );

        modelBuilder.Entity<Component>().HasData(
            new Component
            {
                Id = mouseId,
                Name = "Logitech MX Master 3S",
                Description = "Professional mouse with precision scrolling and customizable buttons. Works with multiple devices.",
                Price = 99.99m,
                Category = ProductCategory.Peripheral,
                ComponentType = ComponentType.Other,
                StockCount = 25,
                CreatedByUserId = seniorEmpId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                AverageRating = 4.7
            }
        );

        // Sample promotion
        var promotionId = Guid.NewGuid();
        modelBuilder.Entity<Promotion>().HasData(new Promotion
        {
            Id = promotionId,
            Name = "Memorial Day Sale",
            DiscountPercent = 10m,
            IsActive = true,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            CreatedByUserId = seniorEmpId,
            CreatedDate = DateTime.UtcNow
        });

        // Link promotion to some products
        modelBuilder.Entity<PromotionProduct>().HasData(
            new PromotionProduct { Id = Guid.NewGuid(), PromotionId = promotionId, ProductId = ramId },
            new PromotionProduct { Id = Guid.NewGuid(), PromotionId = promotionId, ProductId = ssdId },
            new PromotionProduct { Id = Guid.NewGuid(), PromotionId = promotionId, ProductId = mouseId }
        );
    }
}

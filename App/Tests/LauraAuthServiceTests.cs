using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using PCFirmApp.Models;
using PCFirmApp.Services;

namespace PCFirmApp.Tests;

public class LauraAuthServiceTests
{
     private AppDbContext CreateInMemoryContext()
    {
        return new TestAppDbContext();
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldCreateCustomer()
    {
        using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        var result = await authService.RegisterAsync(
            "ana_test",
            "ana@test.com",
            "parola123",
            "Strada Florilor 5",
            "0712345678"
        );

        Assert.NotNull(result);
        Assert.Equal("ana_test", result.Username);
        Assert.Equal("ana@test.com", result.Email);
        Assert.Equal(UserRole.Customer, result.Role);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateUsername_ShouldReturnNull()
    {
        using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        await authService.RegisterAsync("duplicat", "primul@test.com", "parola123");
        var result = await authService.RegisterAsync("duplicat", "altul@test.com", "altaparola");

        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ShouldReturnNull()
    {
        using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        await authService.RegisterAsync("user1", "email@test.com", "parola123");
        var result = await authService.RegisterAsync("user2", "email@test.com", "altaparola");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithCorrectCredentials_ShouldReturnUser()
    {
        using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        await authService.RegisterAsync("login_test", "login@test.com", "parolasecreta");
        var result = await authService.LoginAsync("login_test", "parolasecreta");

        Assert.NotNull(result);
        Assert.Equal("login_test", result.Username);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ShouldReturnNull()
    {
        using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        await authService.RegisterAsync("test_user", "test@test.com", "parolaCORECTA");
        var result = await authService.LoginAsync("test_user", "parolaGRESITA");

        Assert.Null(result);
    }

    [Fact]
    public async Task AddEmployeeAsync_WithManagerRole_ShouldReturnNull()
    {
        using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        var result = await authService.AddEmployeeAsync(
            "fake_manager",
            "fake@test.com",
            "parola123",
            UserRole.Manager
        );

        Assert.Null(result);
    }

    private class TestAppDbContext : AppDbContext
    {
        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        public TestAppDbContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasDiscriminator<string>("UserType")
                .HasValue<Manager>("Manager")
                .HasValue<Employee>("Employee")
                .HasValue<Customer>("Customer");

            modelBuilder.Entity<Product>().HasDiscriminator<string>("ProductType")
                .HasValue<PreAssembledPC>("PreAssembledPC")
                .HasValue<Component>("Component");
        }
    }
}
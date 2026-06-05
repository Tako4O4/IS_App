using System;
using System.Threading.Tasks;
using Xunit;
using PCFirmApp.Models;
using PCFirmApp.Services;

namespace PCFirmApp.Tests;

public class RotaruManagerLoginTests
{
    /// Test 1:
    [Fact]
    public void AppState_AfterManagerLogin_IsManagerShouldBeTrue()
    {
        var appState = new AppState();
        var manager = new Manager
        {
            Id = Guid.NewGuid(),
            Username = "manager",
            Email = "manager@pcfirm.com",
            PasswordHash = "hashed_password",
            Role = UserRole.Manager,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        appState.Login(manager);

        Assert.True(appState.IsLoggedIn);
        Assert.True(appState.IsManager);
        Assert.False(appState.IsEmployee);
        Assert.False(appState.IsSeniorEmployee);
        Assert.False(appState.IsJuniorEmployee);
        Assert.False(appState.IsCustomer);
    }

    /// Test 2:
    [Fact]
    public void AppState_AfterSeniorEmployeeLogin_IsSeniorEmployeeShouldBeTrue()
    {
        var appState = new AppState();
        var seniorEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            Username = "senior_emp",
            Email = "senior@pcfirm.com",
            PasswordHash = "hashed_password",
            Role = UserRole.SeniorEmployee,
            EmploymentDate = DateTime.UtcNow.AddMonths(-6),
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        appState.Login(seniorEmployee);

        Assert.True(appState.IsLoggedIn);
        Assert.True(appState.IsSeniorEmployee);
        Assert.True(appState.IsEmployee);
        Assert.False(appState.IsManager);
        Assert.False(appState.IsJuniorEmployee);
        Assert.False(appState.IsCustomer);
    }

    /// Test 3:
    [Fact]
    public void AppState_AfterJuniorEmployeeLogin_IsJuniorEmployeeShouldBeTrue()
    {
        var appState = new AppState();
        var juniorEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            Username = "junior_emp",
            Email = "junior@pcfirm.com",
            PasswordHash = "hashed_password",
            Role = UserRole.JuniorEmployee,
            EmploymentDate = DateTime.UtcNow.AddMonths(-2),
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        appState.Login(juniorEmployee);

        Assert.True(appState.IsLoggedIn);
        Assert.True(appState.IsJuniorEmployee);
        Assert.True(appState.IsEmployee);
        Assert.False(appState.IsManager);
        Assert.False(appState.IsSeniorEmployee);
        Assert.False(appState.IsCustomer);
    }

    /// Test 4:
    [Fact]
    public void AppState_AfterLogout_ShouldClearCurrentUserAndResetAllFlags()
    {
        var appState = new AppState();
        var manager = new Manager
        {
            Id = Guid.NewGuid(),
            Username = "manager",
            Email = "manager@pcfirm.com",
            PasswordHash = "hashed_password",
            Role = UserRole.Manager,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        appState.Login(manager);
        Assert.True(appState.IsLoggedIn);
        Assert.True(appState.IsManager);

        appState.Logout();

        Assert.False(appState.IsLoggedIn);
        Assert.Null(appState.CurrentUser);
        Assert.False(appState.IsManager);
        Assert.False(appState.IsEmployee);
        Assert.False(appState.IsSeniorEmployee);
        Assert.False(appState.IsJuniorEmployee);
        Assert.False(appState.IsCustomer);
    }

    /// Test 5:
    [Fact]
    public void AppState_WhenLoginWithDifferentUsers_ShouldUpdateRoleFlags()
    {
        var appState = new AppState();
        var manager = new Manager
        {
            Id = Guid.NewGuid(),
            Username = "manager",
            Email = "manager@pcfirm.com",
            PasswordHash = "hashed",
            Role = UserRole.Manager,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Username = "customer1",
            Email = "customer@pcfirm.com",
            PasswordHash = "hashed",
            Role = UserRole.Customer,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        appState.Login(manager);
        Assert.True(appState.IsManager);
        Assert.False(appState.IsCustomer);

        appState.Login(customer);
        Assert.False(appState.IsManager);
        Assert.True(appState.IsCustomer);
    }

    /// Test 6:
    [Fact]
    public void AppState_WithInactiveUser_ShouldStillBeLoggedIn()
    {
        var appState = new AppState();
        var inactiveManager = new Manager
        {
            Id = Guid.NewGuid(),
            Username = "inactive_manager",
            Email = "inactive@pcfirm.com",
            PasswordHash = "hashed_password",
            Role = UserRole.Manager,
            CreatedDate = DateTime.UtcNow,
            IsActive = false  // Inactive user
        };

        appState.Login(inactiveManager);

        Assert.True(appState.IsLoggedIn);
        Assert.True(appState.IsManager);
        Assert.NotNull(appState.CurrentUser);
        Assert.False(appState.CurrentUser.IsActive);
    }
}

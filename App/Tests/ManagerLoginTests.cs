using System;
using System.Threading.Tasks;
using Xunit;
using PCFirmApp.Models;
using PCFirmApp.Services;

namespace PCFirmApp.Tests;

public class ManagerLoginTests
{
    /// <summary>
    /// Test 1: AppState should correctly identify user as Manager after login
    /// </summary>
    [Fact]
    public void AppState_AfterManagerLogin_IsManagerShouldBeTrue()
    {
        // Arrange
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

        // Act
        appState.Login(manager);

        // Assert
        Assert.True(appState.IsLoggedIn);
        Assert.True(appState.IsManager);
        Assert.False(appState.IsEmployee);
        Assert.False(appState.IsSeniorEmployee);
        Assert.False(appState.IsJuniorEmployee);
        Assert.False(appState.IsCustomer);
    }

    /// <summary>
    /// Test 2: AppState should correctly identify user as Senior Employee
    /// </summary>
    [Fact]
    public void AppState_AfterSeniorEmployeeLogin_IsSeniorEmployeeShouldBeTrue()
    {
        // Arrange
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

        // Act
        appState.Login(seniorEmployee);

        // Assert
        Assert.True(appState.IsLoggedIn);
        Assert.True(appState.IsSeniorEmployee);
        Assert.True(appState.IsEmployee);
        Assert.False(appState.IsManager);
        Assert.False(appState.IsJuniorEmployee);
        Assert.False(appState.IsCustomer);
    }

    /// <summary>
    /// Test 3: AppState should correctly identify user as Junior Employee
    /// </summary>
    [Fact]
    public void AppState_AfterJuniorEmployeeLogin_IsJuniorEmployeeShouldBeTrue()
    {
        // Arrange
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

        // Act
        appState.Login(juniorEmployee);

        // Assert
        Assert.True(appState.IsLoggedIn);
        Assert.True(appState.IsJuniorEmployee);
        Assert.True(appState.IsEmployee);
        Assert.False(appState.IsManager);
        Assert.False(appState.IsSeniorEmployee);
        Assert.False(appState.IsCustomer);
    }

    /// <summary>
    /// Test 4: AppState logout should clear current user and reset all role flags
    /// </summary>
    [Fact]
    public void AppState_AfterLogout_ShouldClearCurrentUserAndResetAllFlags()
    {
        // Arrange
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

        // Act
        appState.Logout();

        // Assert
        Assert.False(appState.IsLoggedIn);
        Assert.Null(appState.CurrentUser);
        Assert.False(appState.IsManager);
        Assert.False(appState.IsEmployee);
        Assert.False(appState.IsSeniorEmployee);
        Assert.False(appState.IsJuniorEmployee);
        Assert.False(appState.IsCustomer);
    }

    /// <summary>
    /// Test 5: AppState should properly transition between different user roles
    /// </summary>
    [Fact]
    public void AppState_WhenLoginWithDifferentUsers_ShouldUpdateRoleFlags()
    {
        // Arrange
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

        // Act & Assert - Login as manager
        appState.Login(manager);
        Assert.True(appState.IsManager);
        Assert.False(appState.IsCustomer);

        // Act & Assert - Switch to customer
        appState.Login(customer);
        Assert.False(appState.IsManager);
        Assert.True(appState.IsCustomer);
    }

    /// <summary>
    /// Test 6: AppState should correctly identify inactive users (even though they shouldn't login)
    /// </summary>
    [Fact]
    public void AppState_WithInactiveUser_ShouldStillBeLoggedIn()
    {
        // Arrange
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

        // Act
        appState.Login(inactiveManager);

        // Assert
        Assert.True(appState.IsLoggedIn);  // AppState doesn't check IsActive
        Assert.True(appState.IsManager);
        Assert.NotNull(appState.CurrentUser);
        Assert.False(appState.CurrentUser.IsActive);
    }
}

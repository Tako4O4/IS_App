using Xunit;
using PCFirmApp.Services;

namespace PCFirmApp.Tests;

public class DanuAppStateTests
{
    [Fact]
    public void IsLoggedIn_ShouldReturnFalse_OnInit()
    {
        var appState = new AppState();
        Assert.False(appState.IsLoggedIn);
    }

    [Fact]
    public void SelectedProduct_ShouldBeNull_OnInit()
    {
        var appState = new AppState();
        Assert.Null(appState.SelectedProduct);
    }

    [Fact]
    public void IsManager_ShouldReturnFalse_OnInit()
    {
        var appState = new AppState();
        Assert.False(appState.IsManager);
    }

    [Fact]
    public void IsSeniorEmployee_ShouldReturnFalse_OnInit()
    {
        var appState = new AppState();
        Assert.False(appState.IsSeniorEmployee);
    }

    [Fact]
    public void IsJuniorEmployee_ShouldReturnFalse_OnInit()
    {
        var appState = new AppState();
        Assert.False(appState.IsJuniorEmployee);
    }

    [Fact]
    public void IsCustomer_ShouldReturnFalse_OnInit()
    {
        var appState = new AppState();
        Assert.False(appState.IsCustomer);
    }
}
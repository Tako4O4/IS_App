using Microsoft.Extensions.DependencyInjection;
using PCFirmApp.Services;
using PCFirmApp.ViewModels;
using Xunit;

namespace PCFirmApp.Tests;

public class TestRalu
{
    private class TestViewModel : ViewModelBase
    {
    }

    private class SecondTestViewModel : ViewModelBase
    {
    }

    private NavigationService CreateNavigationService()
    {
        var services = new ServiceCollection();

        services.AddTransient<TestViewModel>();
        services.AddTransient<SecondTestViewModel>();

        var serviceProvider = services.BuildServiceProvider();

        return new NavigationService(serviceProvider);
    }

    [Fact]
    public void CurrentPage_ShouldBeNull_OnInit()
    {
        var navigationService = CreateNavigationService();

        Assert.Null(navigationService.CurrentPage);
    }

    [Fact]
    public void NavigateTo_ShouldSetCurrentPage()
    {
        var navigationService = CreateNavigationService();

        navigationService.NavigateTo<TestViewModel>();

        Assert.NotNull(navigationService.CurrentPage);
    }

    [Fact]
    public void NavigateTo_ShouldSetCorrectViewModelType()
    {
        var navigationService = CreateNavigationService();

        navigationService.NavigateTo<TestViewModel>();

        Assert.IsType<TestViewModel>(navigationService.CurrentPage);
    }

    [Fact]
    public void NavigateToHome_ShouldSetCurrentPageToNull()
    {
        var navigationService = CreateNavigationService();

        navigationService.NavigateToHome();

        Assert.Null(navigationService.CurrentPage);
    }

    [Fact]
    public void NavigateToHome_ShouldClearCurrentPage_AfterNavigation()
    {
        var navigationService = CreateNavigationService();

        navigationService.NavigateTo<TestViewModel>();
        navigationService.NavigateToHome();

        Assert.Null(navigationService.CurrentPage);
    }
}
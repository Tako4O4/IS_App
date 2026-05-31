using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCFirmApp.Services;

namespace PCFirmApp.ViewModels;

public partial class CustomerDashboardViewModel : ViewModelBase
{
    private readonly AppState _appState;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    public CustomerDashboardViewModel(AppState appState, NavigationService navigationService)
    {
        _appState = appState;
        _navigationService = navigationService;
        WelcomeMessage = $"Welcome, {appState.CurrentUser?.Username ?? "Customer"}!";
    }

    [RelayCommand]
    public void BrowseCatalog()
    {
        _navigationService.NavigateTo<ProductCatalogViewModel>();
    }

    [RelayCommand]
    public void MyOrders()
    {
        _navigationService.NavigateTo<CustomerOrdersViewModel>();
    }

    [RelayCommand]
    public void MyServices()
    {
        _navigationService.NavigateTo<CustomerServicesViewModel>();
    }

    [RelayCommand]
    public void ViewCart()
    {
        _navigationService.NavigateTo<CartViewModel>();
    }

    [RelayCommand]
    public void Logout()
    {
        _appState.Logout();
        _navigationService.NavigateTo<LoginViewModel>();
    }
}

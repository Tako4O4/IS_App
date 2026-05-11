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
        // TODO: Navigate to ProductCatalogView in Phase 2
    }

    [RelayCommand]
    public void MyOrders()
    {
        // TODO: Navigate to CustomerOrdersView in Phase 3
    }

    [RelayCommand]
    public void MyServices()
    {
        // TODO: Navigate to CustomerServicesView in Phase 4
    }

    [RelayCommand]
    public void ViewCart()
    {
        // TODO: Navigate to CartView in Phase 3
    }

    [RelayCommand]
    public void Logout()
    {
        _appState.Logout();
        _navigationService.NavigateTo<LoginViewModel>();
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCFirmApp.Services;

namespace PCFirmApp.ViewModels;

public partial class EmployeeDashboardViewModel : ViewModelBase
{
    private readonly AppState _appState;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    [ObservableProperty]
    private bool isSeniorEmployee;

    public EmployeeDashboardViewModel(AppState appState, NavigationService navigationService)
    {
        _appState = appState;
        _navigationService = navigationService;
        WelcomeMessage = $"Welcome, {appState.CurrentUser?.Username ?? "Employee"}!";
        IsSeniorEmployee = appState.IsSeniorEmployee;
    }

    [RelayCommand]
    public void ManageOrders()
    {
        _navigationService.NavigateTo<OrderManagementViewModel>();
    }

    [RelayCommand]
    public void ManageServices()
    {
        _navigationService.NavigateTo<ServiceManagementViewModel>();
    }

    [RelayCommand]
    public void ManageProducts()
    {
        // TODO: Navigate to ProductManagementView in Phase 2 (Senior only)
    }

    [RelayCommand]
    public void ManagePromotions()
    {
        // TODO: Navigate to PromotionManagementView in Phase 6 (Senior only)
    }

    [RelayCommand]
    public void Logout()
    {
        _appState.Logout();
        _navigationService.NavigateTo<LoginViewModel>();
    }
}

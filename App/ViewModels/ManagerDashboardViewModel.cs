using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCFirmApp.Services;

namespace PCFirmApp.ViewModels;

public partial class ManagerDashboardViewModel : ViewModelBase
{
    private readonly AppState _appState;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    public ManagerDashboardViewModel(AppState appState, NavigationService navigationService)
    {
        _appState = appState;
        _navigationService = navigationService;
        WelcomeMessage = $"Welcome, {appState.CurrentUser?.Username ?? "Manager"}!";
    }

    [RelayCommand]
    public void ManageEmployees()
    {
        // TODO: Navigate to EmployeeManagementView in Phase 7
    }

    [RelayCommand]
    public void ViewReports()
    {
        // TODO: Navigate to ReportsView (future phase)
    }

    [RelayCommand]
    public void Logout()
    {
        _appState.Logout();
        _navigationService.NavigateTo<LoginViewModel>();
    }
}

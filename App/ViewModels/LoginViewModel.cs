using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCFirmApp.Services;

namespace PCFirmApp.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly AppState _appState;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isLoading;

    public LoginViewModel(AuthService authService, AppState appState, NavigationService navigationService)
    {
        _authService = authService;
        _appState = appState;
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task Login()
    {
        ErrorMessage = null;
        IsLoading = true;

        try
        {
            var user = await _authService.LoginAsync(Username, Password);
            if (user == null)
            {
                ErrorMessage = "Invalid username or password";
                return;
            }

            _appState.Login(user);

            // Navigate based on role
            if (user.Role == Models.UserRole.Manager)
                _navigationService.NavigateTo<ManagerDashboardViewModel>();
            else if (user.Role == Models.UserRole.SeniorEmployee || user.Role == Models.UserRole.JuniorEmployee)
                _navigationService.NavigateTo<EmployeeDashboardViewModel>();
            else if (user.Role == Models.UserRole.Customer)
                _navigationService.NavigateTo<CustomerDashboardViewModel>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void Register()
    {
        _navigationService.NavigateTo<RegisterViewModel>();
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateToHome();
    }
}

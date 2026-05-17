using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCFirmApp.Services;

namespace PCFirmApp.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly AppState _appState;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string? address;

    [ObservableProperty]
    private string? phoneNumber;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isLoading;

    public RegisterViewModel(AuthService authService, AppState appState, NavigationService navigationService)
    {
        _authService = authService;
        _appState = appState;
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task Register()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Username, email, and password are required";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match";
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters";
            return;
        }

        IsLoading = true;

        try
        {
            var customer = await _authService.RegisterAsync(Username, Email, Password, Address, PhoneNumber);
            if (customer == null)
            {
                ErrorMessage = "Username or email already exists";
                return;
            }

            _appState.Login(customer);
            _navigationService.NavigateTo<CustomerDashboardViewModel>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void Back()
    {
        _navigationService.NavigateTo<LoginViewModel>();
    }
}

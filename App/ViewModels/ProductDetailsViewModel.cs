using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCFirmApp.Services;
using PCFirmApp.Models;

namespace PCFirmApp.ViewModels;

public partial class ProductDetailsViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly AppState _appState;

    // Extragem produsul selectat direct din AppState pentru a-l afișa în interfață
    public Product? Product => _appState.SelectedProduct;

    public ProductDetailsViewModel(NavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateToHome();
    }
}
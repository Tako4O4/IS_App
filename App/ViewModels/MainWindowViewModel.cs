using CommunityToolkit.Mvvm.ComponentModel;
using PCFirmApp.Services;

namespace PCFirmApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly AppState _appState;

    [ObservableProperty]
    private ViewModelBase? currentPage;

    public MainWindowViewModel(NavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;

        _navigationService.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(NavigationService.CurrentPage))
            {
                CurrentPage = _navigationService.CurrentPage;
            }
        };

        // Start with login
        _navigationService.NavigateTo<LoginViewModel>();
    }
}

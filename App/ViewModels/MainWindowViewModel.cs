using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCFirmApp.Services;
using PCFirmApp.Models;

namespace PCFirmApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly AppState _appState;
    private readonly AppDbContext _dbContext;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowProductList))]
    private ViewModelBase? currentPage;

    public bool ShowProductList => CurrentPage == null;

    public ObservableCollection<Product> Products { get; } = new();

    public MainWindowViewModel(NavigationService navigationService, AppState appState, AppDbContext dbContext)
    {
        _navigationService = navigationService;
        _appState = appState;
        _dbContext = dbContext;

        _navigationService.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(NavigationService.CurrentPage))
            {
                CurrentPage = _navigationService.CurrentPage;
            }
        };

        LoadProducts();

        // Start with login
        // _navigationService.NavigateTo<LoginViewModel>();
    }

    private void LoadProducts()
    {
        var products = _dbContext.Products.ToList();
        Products.Clear();
        foreach (var product in products)
        {
            Products.Add(product);
        }
    }

    [RelayCommand]
    private void NavigateToLogin()
    {
        _navigationService.NavigateTo<LoginViewModel>();
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateToHome();
    }
}

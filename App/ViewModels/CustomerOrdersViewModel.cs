using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PCFirmApp.Models;
using PCFirmApp.Services;

namespace PCFirmApp.ViewModels;

public partial class CustomerOrdersViewModel : ViewModelBase
{
    private readonly AppDbContext _context;
    private readonly AppState _appState;
    private readonly NavigationService _navigationService;

    public ObservableCollection<Order> Orders { get; } = new();

    public CustomerOrdersViewModel(AppDbContext context, AppState appState, NavigationService navigationService)
    {
        _context = context;
        _appState = appState;
        _navigationService = navigationService;

        _ = LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        Orders.Clear();

        if (_appState.CurrentUser is not Customer customer)
            return;

        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.CustomerId == customer.Id)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        foreach (var order in orders)
            Orders.Add(order);
    }

    [RelayCommand]
    public void Back()
    {
        _navigationService.NavigateTo<CustomerDashboardViewModel>();
    }
}
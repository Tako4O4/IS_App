using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PCFirmApp.Models;
using PCFirmApp.Services;

namespace PCFirmApp.ViewModels;

public partial class OrderManagementViewModel : ViewModelBase
{
    private readonly AppDbContext _context;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Order> orders = new();

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool hasNoOrders = false;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? successMessage;

    public OrderManagementViewModel(AppDbContext context, NavigationService navigationService)
    {
        _context = context;
        _navigationService = navigationService;

        LoadOrdersCommand.Execute(null);
    }

    [RelayCommand]
    public async Task LoadOrders()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var list = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            Orders.Clear();
            foreach (var order in list)
            {
                Orders.Add(order);
            }

            HasNoOrders = Orders.Count == 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la încărcarea comenzilor: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SetProcessing(Guid orderId)
    {
        await UpdateOrderStatus(orderId, OrderStatus.Processing, "Comanda a fost marcată ca 'În procesare'");
    }

    [RelayCommand]
    public async Task SetReady(Guid orderId)
    {
        await UpdateOrderStatus(orderId, OrderStatus.Ready, "Comanda a fost marcată ca 'Gata'");
    }

    [RelayCommand]
    public async Task SetCompleted(Guid orderId)
    {
        await UpdateOrderStatus(orderId, OrderStatus.Completed, "Comanda a fost finalizată");
    }

    [RelayCommand]
    public async Task SetCancelled(Guid orderId)
    {
        await UpdateOrderStatus(orderId, OrderStatus.Cancelled, "Comanda a fost anulată");
    }

    private async Task UpdateOrderStatus(Guid orderId, OrderStatus newStatus, string successMsg)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                if (newStatus == OrderStatus.Completed)
                {
                    order.DeliveryDate = DateTime.UtcNow;
                }

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                SuccessMessage = successMsg;
                await LoadOrdersCommand.ExecuteAsync(null);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la actualizarea comenzii: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void Back()
    {
        _navigationService.NavigateTo<EmployeeDashboardViewModel>();
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCFirmApp.Models;
using PCFirmApp.Services;

namespace PCFirmApp.ViewModels;

public partial class CartViewModel : ViewModelBase
{
    private readonly CartService _cartService;
    private readonly AppDbContext _context;
    private readonly AppState _appState;
    private readonly NavigationService _navigationService;

    public ObservableCollection<CartItem> Items => _cartService.Items;

    public decimal Total => _cartService.Total;
    public decimal Subtotal => _cartService.Subtotal;
    public decimal TotalDiscount => _cartService.TotalDiscount;

    [ObservableProperty]
    private string? message;

    public CartViewModel(CartService cartService, AppDbContext context, AppState appState, NavigationService navigationService)
    {
        _cartService = cartService;
        _context = context;
        _appState = appState;
        _navigationService = navigationService;

        Items.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Total));
    }

    [RelayCommand]
    public void RemoveItem(CartItem item)
    {
        _cartService.RemoveProduct(item.Product.Id);
        OnPropertyChanged(nameof(Total));
        Message = "Produsul a fost eliminat din coș.";
    }

    [RelayCommand]
    public void ClearCart()
    {
        _cartService.Clear();
        OnPropertyChanged(nameof(Total));
        Message = "Coșul a fost golit.";
    }

    [RelayCommand]
    public async Task PlaceOrder()
    {
        if (_appState.CurrentUser is not Customer customer)
        {
            Message = "Trebuie să fii autentificat ca și client.";
            return;
        }

        if (!Items.Any())
        {
            Message = "Coșul este gol.";
            return;
        }

        var order = new Order
        {
            CustomerId = customer.Id,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            TotalPrice = Total,
            Items = Items.SelectMany(item =>
{
    var orderItems = new List<OrderItem>();

    orderItems.Add(new OrderItem
    {
        ProductId = item.Product.Id,
        Quantity = item.Quantity,
        UnitPrice = item.UnitPrice,
        IsPromotionDiscount = false,
        PromotionLabel = null
    });

    if (item.HasPromotion)
    {
        orderItems.Add(new OrderItem
        {
            ProductId = null,
            Quantity = 1,
            UnitPrice = -item.DiscountValue,
            IsPromotionDiscount = true,
            PromotionLabel = $"Promoție 10% - {item.Product.Name}"
        });
    }

    return orderItems;
}).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        _cartService.Clear();
        OnPropertyChanged(nameof(Total));

        Message = "Comanda a fost plasată cu succes.";
    }

    [RelayCommand]
    public void Back()
    {
        _navigationService.NavigateTo<CustomerDashboardViewModel>();
    }
}
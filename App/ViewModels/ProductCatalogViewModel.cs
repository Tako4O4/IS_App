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

public partial class ProductCatalogViewModel : ViewModelBase
{
    private readonly AppDbContext _context;
    private readonly CartService _cartService;
    private readonly NavigationService _navigationService;
    private readonly AppState _appState;

    public ObservableCollection<Product> Products { get; } = new();

    public ObservableCollection<string> Categories { get; } = new()
    {
        "Toate",
        "DesktopPC",
        "LaptopPC",
        "Printer",
        "Peripheral"
    };

    public ObservableCollection<int> Ratings { get; } = new()
    {
        1, 2, 3, 4, 5
    };

    [ObservableProperty]
    private string selectedCategory = "Toate";

    [ObservableProperty]
    private int selectedRating = 5;

    [ObservableProperty]
    private string reviewText = string.Empty;

    [ObservableProperty]
    private string? message;

    public ProductCatalogViewModel(
        AppDbContext context,
        CartService cartService,
        NavigationService navigationService,
        AppState appState)
    {
        _context = context;
        _cartService = cartService;
        _navigationService = navigationService;
        _appState = appState;

        _ = LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        Products.Clear();

        var query = _context.Products
            .Where(p => p.IsActive);

        if (SelectedCategory != "Toate")
        {
            var category = Enum.Parse<ProductCategory>(SelectedCategory);
            query = query.Where(p => p.Category == category);
        }

        var products = await query.ToListAsync();

        foreach (var product in products)
            Products.Add(product);
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        _ = LoadProductsAsync();
    }

    [RelayCommand]
    public async Task AddToCart(Product product)
    {
        var hasPromotion = await _context.PromotionProducts
            .Include(pp => pp.Promotion)
            .AnyAsync(pp =>
                pp.ProductId == product.Id &&
                pp.Promotion != null &&
                pp.Promotion.IsActive);

        decimal discount = 0;

        if (hasPromotion)
            discount = 10;

        _cartService.AddProduct(product, hasPromotion, discount);

        Message = hasPromotion
            ? $"„{product.Name}” a fost adăugat cu promoție 10%."
            : $"„{product.Name}” a fost adăugat în coș.";
    }

    [RelayCommand]
    public async Task SubmitRating(Product product)
    {
        if (_appState.CurrentUser is not Customer customer)
        {
            Message = "Trebuie să fii autentificat ca și client pentru a lăsa rating.";
            return;
        }

        if (SelectedRating < 1 || SelectedRating > 5)
        {
            Message = "Ratingul trebuie să fie între 1 și 5.";
            return;
        }

        var review = new ProductReview
        {
            ProductId = product.Id,
            CustomerId = customer.Id,
            Rating = SelectedRating,
            ReviewText = string.IsNullOrWhiteSpace(ReviewText) ? null : ReviewText
        };

        _context.ProductReviews.Add(review);
        await _context.SaveChangesAsync();

        var average = await _context.ProductReviews
            .Where(r => r.ProductId == product.Id)
            .AverageAsync(r => r.Rating);

        product.AverageRating = average;
        await _context.SaveChangesAsync();

        ReviewText = string.Empty;
        SelectedRating = 5;
        Message = $"Ratingul pentru „{product.Name}” a fost trimis.";

        await LoadProductsAsync();
    }

    [RelayCommand]
    public void Back()
    {
        _navigationService.NavigateTo<CustomerDashboardViewModel>();
    }
}
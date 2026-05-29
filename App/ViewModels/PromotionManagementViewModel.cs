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

// Helper class for product selection in the form
public partial class SelectableProduct : ViewModelBase
{
    public Guid ProductId { get; set; }
    public string DisplayText { get; set; } = string.Empty;

    [ObservableProperty]
    private bool isSelected;
}

public partial class PromotionManagementViewModel : ViewModelBase
{
    private readonly AppDbContext _context;
    private readonly NavigationService _navigationService;
    private readonly AppState _appState;

    [ObservableProperty]
    private ObservableCollection<Promotion> promotions = new();

    [ObservableProperty]
    private ObservableCollection<SelectableProduct> selectableProducts = new();

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool hasNoPromotions = false;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? successMessage;

    [ObservableProperty]
    private bool isAddFormVisible = false;

    [ObservableProperty]
    private string newName = string.Empty;

    [ObservableProperty]
    private DateTimeOffset newStartDate = DateTimeOffset.Now;

    [ObservableProperty]
    private DateTimeOffset newEndDate = DateTimeOffset.Now.AddMonths(1);

    public PromotionManagementViewModel(AppDbContext context, NavigationService navigationService, AppState appState)
    {
        _context = context;
        _navigationService = navigationService;
        _appState = appState;

        LoadPromotionsCommand.Execute(null);
    }

    [RelayCommand]
    public async Task LoadPromotions()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var list = await _context.Promotions
                .Include(p => p.PromotionProducts)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            Promotions.Clear();
            foreach (var p in list)
            {
                Promotions.Add(p);
            }

            HasNoPromotions = Promotions.Count == 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la încărcarea promoțiilor: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task ShowAddForm()
    {
        ClearFormFields();
        await LoadSelectableProducts();
        IsAddFormVisible = true;
        ErrorMessage = null;
        SuccessMessage = null;
    }

    [RelayCommand]
    public void CancelForm()
    {
        IsAddFormVisible = false;
        ClearFormFields();
        ErrorMessage = null;
    }

    private async Task LoadSelectableProducts()
    {
        try
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            SelectableProducts.Clear();
            foreach (var p in products)
            {
                SelectableProducts.Add(new SelectableProduct
                {
                    ProductId = p.Id,
                    DisplayText = $"{p.Name} ({p.Price:F2} RON)",
                    IsSelected = false
                });
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la încărcarea produselor: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task SavePromotion()
    {
        // Validation
        if (string.IsNullOrWhiteSpace(NewName))
        {
            ErrorMessage = "Numele promoției este obligatoriu";
            return;
        }

        if (NewEndDate <= NewStartDate)
        {
            ErrorMessage = "Data sfârșit trebuie să fie după data început";
            return;
        }

        var selectedProductIds = SelectableProducts
            .Where(sp => sp.IsSelected)
            .Select(sp => sp.ProductId)
            .ToList();

        if (selectedProductIds.Count == 0)
        {
            ErrorMessage = "Selectează cel puțin un produs";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var promotion = new Promotion
            {
                Name = NewName,
                DiscountPercent = 10m,
                IsActive = true,
                StartDate = NewStartDate.UtcDateTime,
                EndDate = NewEndDate.UtcDateTime,
                CreatedByUserId = _appState.CurrentUser?.Id ?? Guid.Empty,
                CreatedDate = DateTime.UtcNow
            };

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            // Link selected products to this promotion
            foreach (var productId in selectedProductIds)
            {
                _context.PromotionProducts.Add(new PromotionProduct
                {
                    PromotionId = promotion.Id,
                    ProductId = productId
                });
            }
            await _context.SaveChangesAsync();

            SuccessMessage = "Promoția a fost adăugată";
            IsAddFormVisible = false;
            ClearFormFields();
            await LoadPromotionsCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la salvarea promoției: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeletePromotion(Guid promotionId)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            // Delete linked products first
            var links = await _context.PromotionProducts
                .Where(pp => pp.PromotionId == promotionId)
                .ToListAsync();
            _context.PromotionProducts.RemoveRange(links);

            // Delete the promotion
            var promotion = await _context.Promotions.FindAsync(promotionId);
            if (promotion != null)
            {
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();

                SuccessMessage = "Promoția a fost ștearsă";
                await LoadPromotionsCommand.ExecuteAsync(null);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la ștergerea promoției: {ex.Message}";
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

    private void ClearFormFields()
    {
        NewName = string.Empty;
        NewStartDate = DateTimeOffset.Now;
        NewEndDate = DateTimeOffset.Now.AddMonths(1);
        foreach (var sp in SelectableProducts)
        {
            sp.IsSelected = false;
        }
    }
}
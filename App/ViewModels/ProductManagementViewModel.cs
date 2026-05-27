using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PCFirmApp.Models;
using PCFirmApp.Services;

namespace PCFirmApp.ViewModels;

public partial class ProductManagementViewModel : ViewModelBase
{
    private readonly AppDbContext _context;
    private readonly NavigationService _navigationService;
    private readonly AppState _appState;

    [ObservableProperty]
    private ObservableCollection<Product> products = new();

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool hasNoProducts = false;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? successMessage;

    [ObservableProperty]
    private bool isAddFormVisible = false;

    [ObservableProperty]
    private bool isEditMode = false;

    [ObservableProperty]
    private string formTitle = "Produs Nou";

    // 0 = PreAssembledPC, 1 = Component
    [ObservableProperty]
    private int productTypeIndex = 0;

    [ObservableProperty]
    private bool isComponentType = false;

    [ObservableProperty]
    private string newName = string.Empty;

    [ObservableProperty]
    private string newDescription = string.Empty;

    [ObservableProperty]
    private string newPrice = "0";

    [ObservableProperty]
    private string newStock = "0";

    [ObservableProperty]
    private string newSpecifications = string.Empty;

    [ObservableProperty]
    private ProductCategory selectedCategory = ProductCategory.DesktopPC;

    [ObservableProperty]
    private ComponentType selectedComponentType = ComponentType.CPU;

    private Guid? _editingProductId = null;

    public List<ProductCategory> AvailableCategories { get; } = Enum.GetValues<ProductCategory>().ToList();
    public List<ComponentType> AvailableComponentTypes { get; } = Enum.GetValues<ComponentType>().ToList();

    public ProductManagementViewModel(AppDbContext context, NavigationService navigationService, AppState appState)
    {
        _context = context;
        _navigationService = navigationService;
        _appState = appState;

        LoadProductsCommand.Execute(null);
    }

    partial void OnProductTypeIndexChanged(int value)
    {
        IsComponentType = (value == 1);
    }

    [RelayCommand]
    public async Task LoadProducts()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var list = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            Products.Clear();
            foreach (var p in list)
            {
                Products.Add(p);
            }

            HasNoProducts = Products.Count == 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la încărcarea produselor: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void ShowAddForm()
    {
        ClearFormFields();
        IsEditMode = false;
        FormTitle = "Produs Nou";
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

    [RelayCommand]
    public async Task SaveProduct()
    {
        // Validation
        if (string.IsNullOrWhiteSpace(NewName) || string.IsNullOrWhiteSpace(NewDescription))
        {
            ErrorMessage = "Numele și descrierea sunt obligatorii";
            return;
        }

        if (!decimal.TryParse(NewPrice, out var price) || price < 0)
        {
            ErrorMessage = "Prețul trebuie să fie un număr pozitiv";
            return;
        }

        if (!int.TryParse(NewStock, out var stock) || stock < 0)
        {
            ErrorMessage = "Stocul trebuie să fie un număr pozitiv";
            return;
        }

        if (NewDescription.Length > 100)
        {
            ErrorMessage = "Descrierea poate avea maxim 100 caractere";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            if (IsEditMode && _editingProductId.HasValue)
            {
                // Update existing product
                var existing = await _context.Products.FindAsync(_editingProductId.Value);
                if (existing != null)
                {
                    existing.Name = NewName;
                    existing.Description = NewDescription;
                    existing.Price = price;
                    existing.StockCount = stock;
                    existing.Category = SelectedCategory;

                    if (existing is PreAssembledPC pc)
                    {
                        pc.Specifications = NewSpecifications;
                    }
                    else if (existing is Component comp)
                    {
                        comp.ComponentType = SelectedComponentType;
                    }

                    _context.Products.Update(existing);
                    await _context.SaveChangesAsync();
                    SuccessMessage = "Produsul a fost actualizat";
                }
            }
            else
            {
                // Add new product
                Product newProduct;
                if (ProductTypeIndex == 0)
                {
                    newProduct = new PreAssembledPC
                    {
                        Name = NewName,
                        Description = NewDescription,
                        Price = price,
                        StockCount = stock,
                        Category = SelectedCategory,
                        Specifications = NewSpecifications,
                        CreatedByUserId = _appState.CurrentUser?.Id
                    };
                }
                else
                {
                    newProduct = new Component
                    {
                        Name = NewName,
                        Description = NewDescription,
                        Price = price,
                        StockCount = stock,
                        Category = SelectedCategory,
                        ComponentType = SelectedComponentType,
                        CreatedByUserId = _appState.CurrentUser?.Id
                    };
                }

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
                SuccessMessage = "Produsul a fost adăugat";
            }

            IsAddFormVisible = false;
            ClearFormFields();
            await LoadProductsCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la salvarea produsului: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task EditProduct(Guid productId)
    {
        try
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return;

            _editingProductId = productId;
            IsEditMode = true;
            FormTitle = "Editează Produs";

            NewName = product.Name;
            NewDescription = product.Description;
            NewPrice = product.Price.ToString();
            NewStock = product.StockCount.ToString();
            SelectedCategory = product.Category;

            if (product is PreAssembledPC pc)
            {
                ProductTypeIndex = 0;
                NewSpecifications = pc.Specifications ?? string.Empty;
            }
            else if (product is Component comp)
            {
                ProductTypeIndex = 1;
                SelectedComponentType = comp.ComponentType;
            }

            IsAddFormVisible = true;
            ErrorMessage = null;
            SuccessMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la încărcarea produsului: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task DeleteProduct(Guid productId)
    {
        try
        {
            IsLoading = true;
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                // Soft delete - mark as inactive
                product.IsActive = false;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                SuccessMessage = "Produsul a fost șters";
                await LoadProductsCommand.ExecuteAsync(null);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la ștergerea produsului: {ex.Message}";
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
        NewDescription = string.Empty;
        NewPrice = "0";
        NewStock = "0";
        NewSpecifications = string.Empty;
        SelectedCategory = ProductCategory.DesktopPC;
        SelectedComponentType = ComponentType.CPU;
        ProductTypeIndex = 0;
        _editingProductId = null;
    }
}
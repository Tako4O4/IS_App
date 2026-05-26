using System.Collections.ObjectModel; // Adăugat pentru coșul de cumpărături
using CommunityToolkit.Mvvm.ComponentModel;
using PCFirmApp.Models;

namespace PCFirmApp.Services;

public partial class AppState : ObservableObject
{
    // --- CODUL TĂU EXISTENT (Rămâne neatins și în siguranță) ---
    [ObservableProperty]
    private User? currentUser;

    public bool IsLoggedIn => CurrentUser != null;

    public bool IsManager => CurrentUser?.Role == UserRole.Manager;
    public bool IsSeniorEmployee => CurrentUser?.Role == UserRole.SeniorEmployee;
    public bool IsJuniorEmployee => CurrentUser?.Role == UserRole.JuniorEmployee;
    public bool IsCustomer => CurrentUser?.Role == UserRole.Customer;
    public bool IsEmployee => IsSeniorEmployee || IsJuniorEmployee;

    public void Login(User user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }


    public Product? SelectedProduct { get; set; }

    // Lista globală care ține minte ce produse au fost adăugate în coș
    public ObservableCollection<Product> Cart { get; } = new();
}
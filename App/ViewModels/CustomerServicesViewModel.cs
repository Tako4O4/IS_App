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

public partial class CustomerServicesViewModel : ViewModelBase
{
    private readonly AppDbContext _context;
    private readonly AppState _appState;
    private readonly NavigationService _navigationService;

    public ObservableCollection<ServiceRequest> ServiceRequests { get; } = new();

    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string contactPhone = string.Empty;
    [ObservableProperty] private string contactEmail = string.Empty;
    [ObservableProperty] private DateTime? dropoffDate = DateTime.Now.AddDays(1);
    [ObservableProperty] private string? message;

    public CustomerServicesViewModel(AppDbContext context, AppState appState, NavigationService navigationService)
    {
        _context = context;
        _appState = appState;
        _navigationService = navigationService;
        _ = LoadRequestsAsync();
    }

    private async Task LoadRequestsAsync()
    {
        ServiceRequests.Clear();

        if (_appState.CurrentUser is not Customer customer)
            return;

        var requests = await _context.ServiceRequests
            .Where(r => r.CustomerId == customer.Id)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();

        foreach (var request in requests)
            ServiceRequests.Add(request);
    }

    [RelayCommand]
    public async Task SubmitRequest()
    {
        if (_appState.CurrentUser is not Customer customer)
        {
            Message = "Trebuie să fii autentificat ca și client.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(ContactPhone) ||
            string.IsNullOrWhiteSpace(ContactEmail))
        {
            Message = "Completează descrierea, telefonul și emailul.";
            return;
        }

        var request = new ServiceRequest
        {
            CustomerId = customer.Id,
            Description = Description,
            ContactPhone = ContactPhone,
            ContactEmail = ContactEmail,
            DropoffDate = DropoffDate ?? DateTime.Now,
            Status = ServiceStatus.Pending,
            RequestDate = DateTime.UtcNow
        };

        _context.ServiceRequests.Add(request);
        await _context.SaveChangesAsync();

        Description = string.Empty;
        ContactPhone = string.Empty;
        ContactEmail = string.Empty;
        DropoffDate = DateTime.Now.AddDays(1);

        Message = "Cererea de service a fost trimisă cu succes.";

        await LoadRequestsAsync();
    }

    [RelayCommand]
    public void Back()
    {
        _navigationService.NavigateTo<CustomerDashboardViewModel>();
    }
}
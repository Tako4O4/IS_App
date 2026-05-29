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

public partial class ServiceManagementViewModel : ViewModelBase
{
    private readonly AppDbContext _context;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<ServiceRequest> serviceRequests = new();

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool hasNoRequests = false;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? successMessage;

    public ServiceManagementViewModel(AppDbContext context, NavigationService navigationService)
    {
        _context = context;
        _navigationService = navigationService;

        LoadServiceRequestsCommand.Execute(null);
    }

    [RelayCommand]
    public async Task LoadServiceRequests()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var list = await _context.ServiceRequests
                .Include(sr => sr.Customer)
                .OrderByDescending(sr => sr.RequestDate)
                .ToListAsync();

            ServiceRequests.Clear();
            foreach (var request in list)
            {
                ServiceRequests.Add(request);
            }

            HasNoRequests = ServiceRequests.Count == 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la încărcarea cererilor: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SetInProgress(Guid requestId)
    {
        await UpdateServiceStatus(requestId, ServiceStatus.InProgress, "Cererea a fost marcată ca 'În lucru'");
    }

    [RelayCommand]
    public async Task SetCompleted(Guid requestId)
    {
        await UpdateServiceStatus(requestId, ServiceStatus.Completed, "Cererea a fost finalizată");
    }

    [RelayCommand]
    public async Task SetCancelled(Guid requestId)
    {
        await UpdateServiceStatus(requestId, ServiceStatus.Cancelled, "Cererea a fost anulată");
    }

    private async Task UpdateServiceStatus(Guid requestId, ServiceStatus newStatus, string successMsg)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            var request = await _context.ServiceRequests.FindAsync(requestId);
            if (request != null)
            {
                request.Status = newStatus;

                _context.ServiceRequests.Update(request);
                await _context.SaveChangesAsync();

                SuccessMessage = successMsg;
                await LoadServiceRequestsCommand.ExecuteAsync(null);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la actualizarea cererii: {ex.Message}";
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
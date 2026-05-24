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

public partial class EmployeeManagementViewModel : ViewModelBase
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    private readonly NavigationService _navigationService;
    private readonly AppState _appState;

    [ObservableProperty]
    private ObservableCollection<Employee> employees = new();

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? successMessage;

    [ObservableProperty]
    private bool isAddFormVisible = false;

    [ObservableProperty]
    private string newUsername = string.Empty;

    [ObservableProperty]
    private string newEmail = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string newConfirmPassword = string.Empty;

    [ObservableProperty]
    private UserRole selectedRole = UserRole.SeniorEmployee;

    public List<UserRole> AvailableRoles { get; } = [UserRole.SeniorEmployee, UserRole.JuniorEmployee];

    public EmployeeManagementViewModel(AppDbContext context, AuthService authService, NavigationService navigationService, AppState appState)
    {
        _context = context;
        _authService = authService;
        _navigationService = navigationService;
        _appState = appState;

        LoadEmployeesCommand.Execute(null);
    }

    [RelayCommand]
    public async Task LoadEmployees()
    {
        try
        {
            IsLoading = true;
            var list = await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.Username)
                .ToListAsync();

            Employees.Clear();
            foreach (var emp in list)
            {
                Employees.Add(emp);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la încărcarea angajaților: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void ShowAddForm()
    {
        IsAddFormVisible = true;
        ErrorMessage = null;
        SuccessMessage = null;
    }

    [RelayCommand]
    public void CancelAdd()
    {
        IsAddFormVisible = false;
        ClearFormFields();
        ErrorMessage = null;
        SuccessMessage = null;
    }

    [RelayCommand]
    public async Task AddEmployee()
    {
        // Validate all fields are non-empty
        if (string.IsNullOrWhiteSpace(NewUsername) ||
            string.IsNullOrWhiteSpace(NewEmail) ||
            string.IsNullOrWhiteSpace(NewPassword) ||
            string.IsNullOrWhiteSpace(NewConfirmPassword))
        {
            ErrorMessage = "Toate câmpurile sunt obligatorii";
            return;
        }

        // Validate passwords match
        if (NewPassword != NewConfirmPassword)
        {
            ErrorMessage = "Parolele nu se potrivesc";
            return;
        }

        // Validate password length
        if (NewPassword.Length < 6)
        {
            ErrorMessage = "Parola trebuie să aibă cel puțin 6 caractere";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            var newEmployee = await _authService.AddEmployeeAsync(
                NewUsername,
                NewEmail,
                NewPassword,
                SelectedRole
            );

            if (newEmployee == null)
            {
                ErrorMessage = "Utilizatorul sau e-mailul există deja";
                return;
            }

            SuccessMessage = "Angajatul a fost adăugat cu succes";
            ClearFormFields();
            IsAddFormVisible = false;

            // Reload the list to show the new employee
            await LoadEmployeesCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la adăugarea angajatului: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteEmployee(Guid employeeId)
    {
        try
        {
            IsLoading = true;
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                await LoadEmployeesCommand.ExecuteAsync(null);
                SuccessMessage = "Angajatul a fost șters cu succes";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la ștergerea angajatului: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task PromoteEmployee(Guid employeeId)
    {
        try
        {
            IsLoading = true;
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                // Toggle between junior and senior
                if (employee.Role == UserRole.JuniorEmployee)
                {
                    employee.Role = UserRole.SeniorEmployee;
                }
                else if (employee.Role == UserRole.SeniorEmployee)
                {
                    employee.Role = UserRole.JuniorEmployee;
                }

                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                await LoadEmployeesCommand.ExecuteAsync(null);

                var newRole = employee.Role == UserRole.SeniorEmployee ? "Senior" : "Junior";
                SuccessMessage = $"Angajatul a fost promovat la {newRole}";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Eroare la modificarea rolului: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void Back()
    {
        // Manager logs out and returns to login
        _appState.Logout();
        _navigationService.NavigateTo<LoginViewModel>();
    }

    private void ClearFormFields()
    {
        NewUsername = string.Empty;
        NewEmail = string.Empty;
        NewPassword = string.Empty;
        NewConfirmPassword = string.Empty;
        SelectedRole = UserRole.SeniorEmployee;
    }
}

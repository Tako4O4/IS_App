using System;
using System.Globalization;
using Avalonia.Data.Converters;
using PCFirmApp.Models;

namespace PCFirmApp.Views;

public class UserRoleConverter : IValueConverter
{
    public static readonly UserRoleConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is UserRole role)
        {
            return role switch
            {
                UserRole.Manager => "Manager",
                UserRole.SeniorEmployee => "Angajat Senior",
                UserRole.JuniorEmployee => "Angajat Junior",
                UserRole.Customer => "Client",
                _ => role.ToString()
            };
        }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

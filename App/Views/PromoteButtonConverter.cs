using System;
using System.Globalization;
using Avalonia.Data.Converters;
using PCFirmApp.Models;

namespace PCFirmApp.Views;

public class PromoteButtonConverter : IValueConverter
{
    public static readonly PromoteButtonConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is UserRole role)
        {
            return role switch
            {
                UserRole.JuniorEmployee => "Promovare",
                UserRole.SeniorEmployee => "Depromovare",
                _ => "Modifică"
            };
        }

        return "Modifică";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

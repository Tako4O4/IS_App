using System;

namespace PCFirmApp.Models;

public enum UserRole
{
    Manager,
    SeniorEmployee,
    JuniorEmployee,
    Customer
}

public enum OrderStatus
{
    Pending,
    Processing,
    Ready,
    Completed,
    Cancelled
}

public enum ProductCategory
{
    DesktopPC,
    LaptopPC,
    Printer,
    Peripheral
}
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

public enum ServiceStatus
{
    Pending,
    InProgress,
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

public enum ComponentType
{
    CPU,
    GPU,
    RAM,
    SSD,
    HDD,
    Motherboard,
    PSU,
    Case,
    Cooler,
    Other
}

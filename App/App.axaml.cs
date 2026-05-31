using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PCFirmApp.Services;
using PCFirmApp.ViewModels;
using PCFirmApp.Views;

namespace PCFirmApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Set up DI container
        var services = new ServiceCollection();

        // Database and core services
        services.AddDbContext<AppDbContext>();
        services.AddSingleton<AppState>();
        services.AddSingleton<NavigationService>();
        services.AddSingleton<CartService>();
        services.AddTransient<ProductCatalogViewModel>();
        services.AddTransient<CartViewModel>();
        services.AddTransient<CustomerOrdersViewModel>();
        services.AddTransient<CustomerServicesViewModel>();

        // Auth service
        services.AddScoped<AuthService>();

        // ViewModels (transient so each navigation gets a fresh instance)
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<EmployeeManagementViewModel>();
        services.AddTransient<EmployeeDashboardViewModel>();
        services.AddTransient<CustomerDashboardViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ProductDetailsViewModel>();
        services.AddTransient<OrderManagementViewModel>();
        services.AddTransient<ServiceManagementViewModel>();
        services.AddTransient<ProductManagementViewModel>();
        services.AddTransient<PromotionManagementViewModel>();

        var serviceProvider = services.BuildServiceProvider();

        // Initialize database
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
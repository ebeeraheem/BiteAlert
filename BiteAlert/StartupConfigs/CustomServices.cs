using BiteAlert.Modules.Authentication;
using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.ProductModule;
using BiteAlert.Modules.Utilities;
using BiteAlert.Modules.VendorModule;

namespace BiteAlert.StartupConfigs;

public static class CustomServices
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        // Add custom services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<UserContextService>();
    }
}

using BiteAlert.Modules.Authentication;
using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.ProductModule;
using BiteAlert.Modules.Utilities;
using BiteAlert.Modules.VendorModule;

namespace BiteAlert.StartupConfigs;

public static class CustomServices
{
    public static void AddCustomServices(this WebApplicationBuilder builder)
    {
        // Add custom services
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IVendorService, VendorService>();
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<UserContextService>();
    }
}

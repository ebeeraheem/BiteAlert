using BiteAlert.Modules.Authentication;
using Microsoft.AspNetCore.Identity;

namespace BiteAlert.StartupConfigs;

public static class Seeder
{
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles = ["Admin", "Vendor", "Customer"];

        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role) is false)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}

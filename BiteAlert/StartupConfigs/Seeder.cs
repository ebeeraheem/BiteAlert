// Ignore Spelling: Admin

using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BiteAlert.StartupConfigs;

public static class Seeder
{
    public static async Task SeedRoles(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider
                        .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles = ["Admin", "Vendor", "Customer"];

        foreach (var role in roles.Where(role => roleManager
                                        .RoleExistsAsync(role).Result is false))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }

    public static async Task SeedAdminUser(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        // Create the default admin user
        var user = new ApplicationUser
        {
            FirstName = "Ibrahim",
            LastName = "Suleiman",
            UserName = "ebeeraheem@gmail.com",
            PhoneNumber = "08143660104",
            PhoneNumberConfirmed = true,
            Email = "ebeeraheem@gmail.com",
            EmailConfirmed = true,
            DateOfBirth = DateTime.Parse("Jan 4, 1999", new CultureInfo("en-US")),
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };

        if (!await userManager.Users.AnyAsync(u => u.Email.Equals(user.Email)))
        {
            var result = await userManager.CreateAsync(user, "string!1Q");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}

// Ignore Spelling: Admin

using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace BiteAlert.StartupConfigs;

public static class Seeder
{
    public static async Task SeedRoles(IServiceProvider serviceProvider)
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

    public static async Task SeedAdminUser(IServiceProvider serviceProvider, IConfiguration config, ApplicationDbContext context)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Retrieve configuration values
        var adminEmail = config["SeedData:Email"];
        var adminFirstName = config["SeedData:FirstName"];
        var adminLastName = config["SeedData:LastName"];
        var adminUserName = config["SeedData:UserName"];
        var adminPassword = config["SeedData:Password"];
        var adminPhoneNumber = config["SeedData:PhoneNumber"];
        var adminDateOfBirth = config["SeedData:DateOfBirth"];

        // Check if email is provided
        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            // Log: Email is required but not provided
            return;
        }

        // Check if the admin user already exists
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin != null)
        {
            // Log: Admin user already exists
            return;
        }

        // Ensure all required fields are provided
        if (string.IsNullOrWhiteSpace(adminFirstName) ||
            string.IsNullOrWhiteSpace(adminLastName) ||
            string.IsNullOrWhiteSpace(adminUserName) ||
            string.IsNullOrWhiteSpace(adminPassword) ||
            string.IsNullOrWhiteSpace(adminPhoneNumber) ||
            string.IsNullOrWhiteSpace(adminDateOfBirth))
        {
            // Log: Insufficient information to create a new admin
            return;
        }

        // Create the new admin user
        var user = new ApplicationUser
        {
            FirstName = adminFirstName,
            LastName = adminLastName,
            UserName = adminUserName,
            PhoneNumber = adminPhoneNumber,
            PhoneNumberConfirmed = true,
            Email = adminEmail,
            EmailConfirmed = true,
            DateOfBirth = DateTime.Parse(adminDateOfBirth, new CultureInfo("en-US")),
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
            await context.SaveChangesAsync();
            // Log: Admin user created and assigned role successfully
        }
        else
        {
            // Log: Failed to create admin user, reason(s): {string.Join(", ", result.Errors.Select(e => e.Description))}
        }
    }
}

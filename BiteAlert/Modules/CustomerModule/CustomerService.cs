using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Modules.CustomerModule;

public class CustomerService(UserManager<ApplicationUser> userManager,
                             ApplicationDbContext context,
                             ILogger<CustomerService> logger) : ICustomerService
{
    public async Task<Customer?> GetCustomerById(string userId)
    {
        return await context.Customers.SingleOrDefaultAsync(
            u => u.Id.ToString() == userId);
    }

    public async Task<UpsertCustomerResponse> RegisterCustomerAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found.", userId);
            return new UpsertCustomerResponse()
            {
                Succeeded = false,
                Message = "User not found."
            };
        }

        var isCustomer = await userManager.IsInRoleAsync(user, "Customer");
        if (isCustomer is false)
        {
            logger.LogWarning("User is not a customer. User ID: {Id}", userId);
            return new UpsertCustomerResponse()
            {
                Succeeded = false,
                Message = "User is not a customer."
            };
        }

        var customer = new Customer()
        {
            Id = user.Id,
            User = user
        };

        user.LastUpdatedAt = DateTime.UtcNow;

        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        return new UpsertCustomerResponse()
        {
            Succeeded = true,
            Message = "Customer registered successfully."
        };
    }
}

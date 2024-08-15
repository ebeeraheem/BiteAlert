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
        logger.LogInformation("Searching for customer by Id {Id}", userId);

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

        var isValidGuid = Guid.TryParse(userId, out Guid userGuid);

        if (!isValidGuid)
        {
            logger.LogWarning("Invalid user Id: {Id}", userId);

            return new UpsertCustomerResponse()
            {
                Succeeded = false,
                Message = "Invalid user id."
            };
        }

        // Check if the user is a vendor
        var userIsVendor = await context.Vendors.FindAsync(userGuid);

        if (userIsVendor is not null)
        {
            logger.LogWarning("User {Id} is already registered as a vendor.", userGuid);

            return new UpsertCustomerResponse()
            {
                Succeeded = false,
                Message = "You are currently registered as a vendor."
            };
        }

        // Check if the user is already a customer
        var userIsCustomer = await context.Customers.FindAsync(userGuid);

        if (userIsCustomer is not null)
        {
            logger.LogWarning("User {Id} is already registered as a customer.", userGuid);

            return new UpsertCustomerResponse()
            {
                Succeeded = false,
                Message = "User is already a customer."
            };
        }

        var customer = new Customer()
        {
            Id = user.Id,
            User = user
        };

        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        return new UpsertCustomerResponse()
        {
            Succeeded = true,
            Message = "Customer registered successfully."
        };
    }
}

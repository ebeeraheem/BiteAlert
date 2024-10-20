using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Shared;
using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.CustomerModule;

public class CustomerService(UserManager<ApplicationUser> userManager,
                             ApplicationDbContext context,
                             ILogger<CustomerService> logger) : ICustomerService
{
    public async Task<BaseResponse> GetCustomerById(string userId)
    {
        var customer = await context.Customers.FindAsync(userId);

        if (customer is null)
        {
            logger.LogWarning("Customer not found. ID: {Id}", userId);
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Customer not found.",
                Data = new {  userId }
            };
        }

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Success.",
            Data = new { customer }
        };
    }

    public async Task<BaseResponse> RegisterCustomerAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found.", userId);
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User not found.",
                Data = new { userId }
            };
        }

        var isCustomer = await userManager.IsInRoleAsync(user, "Customer");
        if (isCustomer is false)
        {
            logger.LogWarning("User is not a customer. User ID: {Id}", userId);
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User is not a customer.",
                Data = new { userId }
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

        // TODO: Create a customer response dto
        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Customer registered successfully.",
            Data = new { customer }
        };
    }
}

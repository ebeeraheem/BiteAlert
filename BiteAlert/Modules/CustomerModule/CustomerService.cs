﻿using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Authentication;
using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.CustomerModule;

public class CustomerService : ICustomerService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public CustomerService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<UpsertCustomerResponse> RegisterCustomerAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return new UpsertCustomerResponse()
            {
                Succeeded = false,
                Message = "user not found"
            };
        }

        var isValidGuid = Guid.TryParse(userId, out Guid userGuid);

        if (!isValidGuid)
        {
            return new UpsertCustomerResponse()
            {
                Succeeded = false,
                Message = "invalid user id"
            };
        }

        var userIsCustomer = await _context.Customers.FindAsync(userGuid);

        if (userIsCustomer is not null)
        {
            return new UpsertCustomerResponse()
            {
                Succeeded = false,
                Message = "user is already a customer"
            };
        }

        var customer = new Customer()
        {
            Id = user.Id,
            User = user
        };

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();

        return new UpsertCustomerResponse()
        {
            Succeeded = true,
            Message = "customer registered successfully"
        };
    }
}

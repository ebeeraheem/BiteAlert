﻿using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Modules.VendorModule;

public class VendorService(UserManager<ApplicationUser> userManager,
                           ApplicationDbContext context,
                           ILogger<VendorService> logger) : IVendorService
{
    public async Task<UpsertVendorResponse> RegisterVendorAsync(string userId, UpsertVendorRequest request)
    {
        var transaction = await context.Database
            .BeginTransactionAsync();

        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User with Id {Id} not found.", userId);
                return new UpsertVendorResponse()
                {
                    Succeeded = false,
                    Message = "User not found."
                };
            }

            var isVendor = await userManager.IsInRoleAsync(user, "Vendor");
            if (isVendor is false)
            {
                logger.LogWarning("User is not a vendor. User ID: {Id}", userId);
                return new UpsertVendorResponse()
                {
                    Succeeded = false,
                    Message = "User is not a vendor."
                };
            }

            // Check whether email is verified
            var isVerified = user.EmailConfirmed;
            if (isVerified is false)
            {
                logger.LogWarning("Vendor email is not verified. Email: {Email}", user.Email);
                return new UpsertVendorResponse()
                {
                    Succeeded = false,
                    Message = "Vendor email is not verified."
                };
            }

            // Create a new vendor
            var vendor = new Vendor
            {
                Id = user.Id,
                User = user,
                BusinessName = request.BusinessName,
                BusinessTagline = request.BusinessTagline,
                BusinessDescription = request.BusinessDescription,
                BusinessAddress = request.BusinessAddress,
                BusinessEmail = request.BusinessEmail,
                BusinessPhoneNumber = request.BusinessPhoneNumber
            };

            // Save the vendor
            await context.Vendors.AddAsync(vendor);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            var response = new UpsertVendorResponse()
            {
                Succeeded = true,
                Message = "Vendor registered successfully."
            };

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred during vendor registration. ID: {Id}",
                        userId);

            await transaction.RollbackAsync();
            throw;
        }
    }

    // Get vendor information
    public async Task<Vendor?> GetVendorByIdAsync(string vendorId)
    {
        return await context.Vendors
            .SingleOrDefaultAsync(v => v.Id.ToString() == vendorId);
    }

    public async Task<Vendor?> GetVendorByUserNameAsync(string userName)
    {
        return await context.Vendors
            .SingleOrDefaultAsync(v => v.User.UserName == userName);
    }

    // Update vendor information
    public async Task<UpsertVendorResponse> UpdateVendorBusinessInfo(string vendorId, UpsertVendorRequest request)
    {
        var transaction = await context.Database
            .BeginTransactionAsync();

        try
        {
            var vendor = await context.Vendors.SingleOrDefaultAsync(
                                    v => v.Id.ToString() == vendorId);

            if (vendor is null)
            {
                logger.LogWarning("Vendor with Id {Id} not found.", vendorId);
                return new UpsertVendorResponse()
                {
                    Succeeded = false,
                    Message = "Vendor not found."
                };
            }

            // Update only the fields provided in the request
            logger.LogInformation("Updating business information for vendor {Id}", vendorId);

            if (request.BusinessName is not null)
                vendor.BusinessName = request.BusinessName;

            if (request.BusinessTagline is not null)
                vendor.BusinessTagline = request.BusinessTagline;

            if (request.BusinessDescription is not null)
                vendor.BusinessDescription = request.BusinessDescription;

            if (request.BusinessAddress is not null)
                vendor.BusinessAddress = request.BusinessAddress;

            if (request.BusinessEmail is not null)
                vendor.BusinessEmail = request.BusinessEmail;

            if (request.BusinessPhoneNumber is not null)
                vendor.BusinessPhoneNumber = request.BusinessPhoneNumber;

            context.Vendors.Update(vendor);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            var response = new UpsertVendorResponse()
            {
                Succeeded = true,
                Message = "Vendor business info updated successfully."
            };

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while updating business information for vendor {VendorId}",
                        vendorId);

            await transaction.RollbackAsync();
            throw;
        }
    }
}

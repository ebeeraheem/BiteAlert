using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Shared;
using BiteAlert.Modules.Shared.PagedResult;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Modules.VendorModule;

public class VendorService(UserManager<ApplicationUser> userManager,
                           ApplicationDbContext context,
                           IPagedResultService pagedResultService,
                           ILogger<VendorService> logger) : IVendorService
{
    public async Task<BaseResponse> RegisterVendorAsync(string userId, UpsertVendorRequest request)
    {
        var transaction = await context.Database
            .BeginTransactionAsync();

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found.", userId);
            await transaction.RollbackAsync();
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User not found.",
                Data = new { userId }
            };
        }

        var isVendor = await userManager.IsInRoleAsync(user, "Vendor");
        if (isVendor is false)
        {
            logger.LogWarning("User is not a vendor. User ID: {Id}", userId);
            await transaction.RollbackAsync();
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User is not a vendor.",
                Data = new { userId }
            };
        }

        // Check whether email is verified
        var isVerified = user.EmailConfirmed;
        if (isVerified is false)
        {
            logger.LogWarning("Vendor email is not verified. Email: {Email}", user.Email);
            await transaction.RollbackAsync();
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Vendor email is not verified.",
                Data = new { userId }
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

        user.LastUpdatedAt = DateTime.UtcNow;

        // Save the vendor
        await context.Vendors.AddAsync(vendor);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Vendor registered successfully.",
            Data = new { vendor }
        };
    }

    // Get vendor information
    public async Task<BaseResponse> GetVendorByIdAsync(string vendorId)
    {
        var vendor = await context.Vendors.FindAsync(vendorId);

        if (vendor is null)
        {
            logger.LogWarning("Vendor not found. ID: {Id}", vendorId);
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Vendor not found.",
                Data = new { vendorId }
            };
        }

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Success.",
            Data = new { vendor }
        };
    }

    public async Task<BaseResponse> GetVendorsByUserNameAsync(string userName, int pageNumber, int pageSize)
    {
        var query = context.Vendors
            .Where(v => v.User.UserName.Contains(userName));

        var pagedResult = await pagedResultService.GetPagedResultAsync(query, pageNumber, pageSize);

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Success.",
            Data = new { pagedResult }
        };
    }

    public async Task<BaseResponse> UpdateVendorBusinessInfo(string vendorId, UpsertVendorRequest request)
    {
        var transaction = await context.Database
            .BeginTransactionAsync();

        var vendor = await context.Vendors
                        .Include(v => v.User)
                        .SingleOrDefaultAsync(v =>
                                v.Id.ToString() == vendorId);

        if (vendor is null)
        {
            logger.LogWarning("Vendor with Id {Id} not found.", vendorId);
            await transaction.RollbackAsync();
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Vendor not found.",
                Data = new { vendorId }
            };
        }

        // Update only the fields provided in the request
        logger.LogInformation("Updating business information for vendor {Id}", vendorId);

        if (string.IsNullOrWhiteSpace(request.BusinessName) is false)
            vendor.BusinessName = request.BusinessName;

        if (string.IsNullOrWhiteSpace(request.BusinessTagline) is false)
            vendor.BusinessTagline = request.BusinessTagline;

        if (string.IsNullOrWhiteSpace(request.BusinessDescription) is false)
            vendor.BusinessDescription = request.BusinessDescription;

        if (string.IsNullOrWhiteSpace(request.BusinessAddress) is false)
            vendor.BusinessAddress = request.BusinessAddress;

        if (string.IsNullOrWhiteSpace(request.BusinessEmail) is false)
            vendor.BusinessEmail = request.BusinessEmail;

        if (string.IsNullOrWhiteSpace(request.BusinessPhoneNumber) is false)
            vendor.BusinessPhoneNumber = request.BusinessPhoneNumber;

        vendor.User.LastUpdatedAt = DateTime.UtcNow;

        context.Vendors.Update(vendor);
        await context.SaveChangesAsync();

        await transaction.CommitAsync();

        return new UpsertVendorResponse()
        {
            Succeeded = true,
            Message = "Vendor business info updated successfully.",
            Data = new { vendorId }
        };
    }
}

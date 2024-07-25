using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Authentication;
using BiteAlert.Modules.CustomerModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Modules.VendorModule;

public class VendorService : IVendorService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public VendorService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // Register a vendor
    public async Task<UpsertVendorResponse> RegisterVendorAsync(string userId,            UpsertVendorRequest request)
    {
        var transaction = await _context.Database
            .BeginTransactionAsync();

        try
        {
            // Find the user with the specified id
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return new UpsertVendorResponse()
                {
                    Succeeded = false,
                    Message = "user not found"
                };
            }

            var isValidGuid = Guid.TryParse(userId, out Guid userGuid);

            if (!isValidGuid)
            {
                return new UpsertVendorResponse()
                {
                    Succeeded = false,
                    Message = "invalid user id"
                };
            }

            // Check if the user is a customer
            var userIsCustomer = await _context.Customers.FindAsync(userGuid);

            if (userIsCustomer is not null)
            {
                return new UpsertVendorResponse()
                {
                    Succeeded = false,
                    Message = "You are currently registered as a customer."
                };
            }

            // Check if user is already a vendor
            var userIsVendor = await _context.Vendors.FindAsync(userGuid);
            if (userIsVendor is not null)
            {
                return new UpsertVendorResponse()
                {
                    Succeeded = false,
                    Message = "user is already a vendor"
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
            await _context.Vendors.AddAsync(vendor);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var response = new UpsertVendorResponse()
            {
                Succeeded = true,
                Message = "vendor registered successfully"
            };

            return response;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Get vendor information
    public async Task<Vendor?> GetVendorByIdAsync(string vendorId)
    {
        return await _context.Vendors
            .SingleOrDefaultAsync(v => v.Id.ToString() == vendorId);
    }

    public async Task<Vendor?> GetVendorByUserNameAsync(string userName)
    {
        return await _context.Vendors
            .SingleOrDefaultAsync(v => v.User.UserName == userName);
    }

    // Update vendor information
    public async Task<UpsertVendorResponse> UpdateVendorBusinessInfo(string vendorId, UpsertVendorRequest request)
    {
        var transaction = await _context.Database
            .BeginTransactionAsync();

        try
        {
            var vendor = await _context.Vendors.SingleOrDefaultAsync(
                                    v => v.Id.ToString() == vendorId);

            if (vendor is null)
            {
                return new UpsertVendorResponse()
                {
                    Succeeded = false,
                    Message = "vendor not found"
                };
            }

            // Update only the fields provided in the request
            if (request.BusinessName is not null)
            {
                vendor.BusinessName = request.BusinessName;
            }

            if (request.BusinessTagline is not null)
            {
                vendor.BusinessTagline = request.BusinessTagline;
            }

            if (request.BusinessDescription is not null)
            {
                vendor.BusinessDescription = request.BusinessDescription;
            }

            if (request.BusinessAddress is not null)
            {
                vendor.BusinessAddress = request.BusinessAddress;
            }

            if (request.BusinessEmail is not null)
            {
                vendor.BusinessEmail = request.BusinessEmail;
            }

            if (request.BusinessPhoneNumber is not null)
            {
                vendor.BusinessPhoneNumber = request.BusinessPhoneNumber;
            }

            _context.Vendors.Update(vendor);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var response = new UpsertVendorResponse()
            {
                Succeeded = true,
                Message = "vendor updated successfully"
            };

            return response;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

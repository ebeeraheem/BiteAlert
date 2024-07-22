using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Authentication;
using Microsoft.AspNetCore.Identity;

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
    public async Task<RegisterVendorResponse> RegisterVendorAsync(string userId,            RegisterVendorRequest request)
    {
        var transaction = await _context.Database
            .BeginTransactionAsync();

        try
        {
            // Find the user with the specified id
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return new RegisterVendorResponse()
                {
                    Succeeded = false,
                    Message = "user not found"
                };
            }

            // Check if user is already a vendor
            var userIsVendor = await _context.Vendors.FindAsync(userId);
            if (userIsVendor is not null)
            {
                return new RegisterVendorResponse()
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
                BusinessPhoneNumber = request.BusinessPhoneNumber,
                LogoUrl = request.LogoUrl,
            };

            // Save the vendor
            await _context.Vendors.AddAsync(vendor);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var response = new RegisterVendorResponse()
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
    // Login
    public Task<IdentityResult> LoginVendorAsync()
    {
        throw new NotImplementedException();
    }

    // Get vendor information
    public Task<Vendor> GetVendorByIdAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Vendor> GetVendorByUserNameAsync()
    {
        throw new NotImplementedException();
    }

    // Update vendor information
    public Task UpdateVendorBusinessInfo()
    {
        throw new NotImplementedException();
    }

    public Task UpdateVendorProfileInfo()
    {
        throw new NotImplementedException();
    }

    // Confirm email
    public Task ConfirmEmailAsync()
    {
        throw new NotImplementedException();
    }

    // Delete vendor account
    public Task DeleteVendorAsync()
    {
        throw new NotImplementedException();
    }
}

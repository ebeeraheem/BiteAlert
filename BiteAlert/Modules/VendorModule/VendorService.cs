using BiteAlert.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.VendorModule;

public class VendorService
{
    //private readonly UserManager<Vendor> _userManager;
    //private readonly ApplicationDbContext _context;

    //public VendorService(UserManager<Vendor> userManager, ApplicationDbContext context)
    //{
    //    _userManager = userManager;
    //    _context = context;
    //}

    //// Register a vendor
    //public async Task<IdentityResult> RegisterVendorAsync(VendorRegistrationRequest request)
    //{
    //    var transaction = await _context.Database
    //        .BeginTransactionAsync();

    //    try
    //    {
    //        // Create a new vendor
    //        var vendor = new Vendor
    //        {
    //            //UserName = request.Email,
    //            //Email = request.Email,
    //            BusinessName = request.BusinessName,
    //            BusinessDescription = request.BusinessDescription,
    //            BusinessAddress = request.BusinessAddress
    //        };

    //        // Register the vendor
    //        var result = await _userManager.CreateAsync(vendor, request.Password);

    //        if (result.Succeeded)
    //        {
    //            // Generate email confirmation token

    //            // Generate email confirmation link

    //            // Send email confirmation message
    //        }

    //        await transaction.CommitAsync();
    //        return result;
    //    }
    //    catch (Exception)
    //    {
    //        await transaction.RollbackAsync();
    //        throw;
    //    }
    //}
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

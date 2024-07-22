using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.VendorModule;

public interface IVendorService
{
    Task<IdentityResult> RegisterVendorAsync(VendorRegistrationRequest request);
    Task ConfirmEmailAsync();
    Task<IdentityResult> LoginVendorAsync();
    Task<Vendor> GetVendorByIdAsync();
    Task<Vendor> GetVendorByUserNameAsync();
    Task UpdateVendorProfileInfo();
    Task UpdateVendorBusinessInfo();
    Task DeleteVendorAsync();
}

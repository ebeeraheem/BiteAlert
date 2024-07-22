using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.VendorModule;

public interface IVendorService
{
    Task<RegisterVendorResponse> RegisterVendorAsync(string userId, RegisterVendorRequest request);
    Task ConfirmEmailAsync();
    Task<IdentityResult> LoginVendorAsync();
    Task<Vendor> GetVendorByIdAsync();
    Task<Vendor> GetVendorByUserNameAsync();
    Task UpdateVendorProfileInfo();
    Task UpdateVendorBusinessInfo();
    Task DeleteVendorAsync();
}

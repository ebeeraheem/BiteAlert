namespace BiteAlert.Modules.VendorModule;

public interface IVendorService
{
    Task<RegisterVendorResponse> RegisterVendorAsync(string userId, RegisterVendorRequest request);
    Task ConfirmEmailAsync();
    Task<Vendor?> GetVendorByIdAsync(string vendorId);
    Task<Vendor?> GetVendorByUserNameAsync(string userName);
    Task UpdateVendorProfileInfo();
    Task UpdateVendorBusinessInfo();
    Task DeleteVendorAsync();
}

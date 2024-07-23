namespace BiteAlert.Modules.VendorModule;

public interface IVendorService
{
    Task<UpsertVendorResponse> RegisterVendorAsync(string userId, UpsertVendorRequest request);
    Task ConfirmEmailAsync();
    Task<Vendor?> GetVendorByIdAsync(string vendorId);
    Task<Vendor?> GetVendorByUserNameAsync(string userName);
    Task<UpsertVendorResponse> UpdateVendorBusinessInfo(string vendorId, UpsertVendorRequest request);
    Task DeleteVendorAsync();
}

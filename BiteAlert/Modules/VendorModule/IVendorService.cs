namespace BiteAlert.Modules.VendorModule;

public interface IVendorService
{
    Task<UpsertVendorResponse> RegisterVendorAsync(string userId, UpsertVendorRequest request);
    Task<Vendor?> GetVendorByIdAsync(string vendorId);
    Task<List<Vendor>> GetVendorsByUserNameAsync(string userName);
    Task<UpsertVendorResponse> UpdateVendorBusinessInfo(string vendorId, UpsertVendorRequest request);
}

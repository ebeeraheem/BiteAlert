using BiteAlert.Modules.Shared.PagedResult;

namespace BiteAlert.Modules.VendorModule;

public interface IVendorService
{
    Task<UpsertVendorResponse> RegisterVendorAsync(string userId, UpsertVendorRequest request);
    Task<Vendor?> GetVendorByIdAsync(string vendorId);
    Task<PagedResult<Vendor>> GetVendorsByUserNameAsync(string userName, int pageNumber, int pageSize);
    Task<UpsertVendorResponse> UpdateVendorBusinessInfo(string vendorId, UpsertVendorRequest request);
}

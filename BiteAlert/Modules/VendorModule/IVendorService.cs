using BiteAlert.Modules.Shared;

namespace BiteAlert.Modules.VendorModule;

public interface IVendorService
{
    Task<BaseResponse> RegisterVendorAsync(string userId, UpsertVendorRequest request);
    Task<BaseResponse> GetVendorByIdAsync(string vendorId);
    Task<BaseResponse> GetVendorsByUserNameAsync(string userName, int pageNumber, int pageSize);
    Task<BaseResponse> UpdateVendorBusinessInfo(string vendorId, UpsertVendorRequest request);
}

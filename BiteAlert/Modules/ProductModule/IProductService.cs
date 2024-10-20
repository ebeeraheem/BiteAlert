
using BiteAlert.Modules.Shared;

namespace BiteAlert.Modules.ProductModule;

public interface IProductService
{
    Task<BaseResponse> GetProductByIdAsync(string productId);
    Task<BaseResponse> GetProductsAsync();
    Task<BaseResponse> CreateAsync(string vendorId, UpsertProductRequest request);
    Task<BaseResponse> UpdateAsync(string vendorId, string productId, UpsertProductRequest request);
    Task<BaseResponse> DeleteAsync(string vendorId, string productId);
}
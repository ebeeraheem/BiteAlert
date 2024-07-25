
namespace BiteAlert.Modules.ProductModule;

public interface IProductService
{
    Task<UpsertProductResponse> GetProductByIdAsync(string productId);
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<UpsertProductResponse> CreateAsync(string vendorId, UpsertProductRequest request);
    Task<UpsertProductResponse> UpdateAsync(string vendorId, string productId, UpsertProductRequest request);
    Task<bool> DeleteAsync(string productId);
}
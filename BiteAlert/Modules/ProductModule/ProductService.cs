using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Shared;
using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Modules.ProductModule;

public class ProductService(ApplicationDbContext context, ILogger<ProductService> logger) : IProductService
{
    public async Task<BaseResponse> GetProductByIdAsync(string productId)
    {
        logger.LogInformation("Searching for product with Id {Id}", productId);

        var isValidGuid = Guid.TryParse(productId, out var productGuid);

        if (isValidGuid is false)
        {
            logger.LogWarning("Invalid product Id provided. Id: {Id}", productId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Invalid product id.",
                Data = new { productId }
            };
        }

        var product = await context.Products.FindAsync(productGuid);

        if (product is null)
        {
            logger.LogWarning("Product with Id {Id} not found", productId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Product not found",
                Data = new { productId }
            };
        }

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Success",
            Data = new { product }
        };
    }

    // Get all products
    // Note: Consider changing this to get a single vendor's products
    // TODO: Add filtering and pagination
    public async Task<BaseResponse> GetProductsAsync()
    {
        var products = await context.Products.ToListAsync();

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Success.",
            Data = new { products }
        };
    }

    // Create a new product
    public async Task<BaseResponse> CreateAsync(string vendorId, UpsertProductRequest request)
    {
        // Check if the vendorId is a valid Guid
        bool isValidGuid = Guid.TryParse(vendorId, out Guid vendorGuid);

        if (isValidGuid is false)
        {
            logger.LogWarning("Invalid vendor Id: {Id}", vendorId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Invalid vendor id",
                Data = new { vendorId }
            };
        }

        var transaction = await context.Database
            .BeginTransactionAsync();

        // Get the vendor who is creating the product
        var vendor = await context.Vendors.FindAsync(vendorGuid);

        if (vendor is null)
        {
            logger.LogWarning("vendor with Id {Id} not found", vendorGuid);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Vendor not found",
                Data = new { vendorId }
            };
        }

        // Create a new product
        logger.LogInformation("Creating a new product by vendor with Id {Id}", vendorGuid);

        var product = new Product()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            IsAvailable = false,
            VendorId = vendor.Id,
            Vendor = vendor
        };

        // Save the product
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        await transaction.CommitAsync();

        // TODO: Create a product response dto
        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Product created successfully",
            Data = new { product }
        };
    }

    // Update an existing product
    public async Task<BaseResponse> UpdateAsync(string vendorId,
                                                         string productId,
                                                         UpsertProductRequest request)
    {
        var transaction = await context.Database
            .BeginTransactionAsync();

        // Get the vendor who is updating the product
        var vendor = await context.Vendors
                        .Include(v => v.Products)
                        .SingleOrDefaultAsync(v => v.Id.ToString() == vendorId);

        if (vendor is null)
        {
            logger.LogWarning("Vendor with Id {Id} not found.", vendorId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Vendor not found",
                Data = new { vendorId }
            };
        }

        // Return false if the vendor doesn't have any product
        if (vendor.Products is null)
        {
            logger.LogWarning("Vendor {Id} has no products.", vendorId);
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Vendor has no products",
                Data = new { vendorId }
            };
        }

        // Get the product to be updated
        var product = vendor.Products.SingleOrDefault(
                            p => p.Id.ToString().Equals(productId, 
                            StringComparison.CurrentCultureIgnoreCase));

        if (product is null)
        {
            logger.LogWarning("Product {Id} not found. Vendor ID: {VendorId}",
                        productId,
                        vendorId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Product not found",
                Data = new { vendorId, productId }
            };
        }

        // Update only the fields that are not null
        logger.LogInformation("Vendor {VendorId} updating product {ProductId}", vendorId, productId);

        if (request.Name is not null) product.Name = request.Name;
        if (request.Description is not null) product.Description = request.Description;
        if (request.Price != product.Price) product.Price = request.Price;
        if (request.ImageUrl is not null) product.ImageUrl = request.ImageUrl;

        // Update the product
        context.Products.Update(product);
        await context.SaveChangesAsync();

        await transaction.CommitAsync();

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Product updated successfully.",
            Data = new { product }
        };
    }

    // Delete a vendor's product
    public async Task<BaseResponse> DeleteAsync(string vendorId, string productId)
    {
        var transaction = await context.Database
            .BeginTransactionAsync();

        // Get the vendor who is deleting the product
        var vendor = await context.Vendors
                        .Include(v => v.Products)
                        .SingleOrDefaultAsync(v => v.Id.ToString() == vendorId);

        if (vendor is null)
        {
            logger.LogWarning("Vendor with Id {Id} not found.", vendorId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Vendor not found",
                Data = new { vendorId }
            };
        }

        // Return false if the vendor doesn't have any product
        if (vendor.Products is null)
        {
            logger.LogWarning("Vendor {Id} has no products.", vendorId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Vendor has no products",
                Data = new { vendorId }
            };
        }

        // Get the product to be deleted
        var product = vendor.Products.SingleOrDefault(
                            p => p.Id.ToString().Equals(productId, 
                            StringComparison.CurrentCultureIgnoreCase));

        if (product is null)
        {
            logger.LogWarning("Product {Id} not found. Vendor ID: {VendorId}",
                        productId,
                        vendorId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Product not found",
                Data = new { vendorId, productId }
            };
        }

        // Delete the product
        logger.LogInformation("Vendor {VendorId} deleting product {ProductId}", vendorId, productId);

        context.Products.Remove(product);
        await context.SaveChangesAsync();

        await transaction.CommitAsync();

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Product deleted successfully.",
            Data = new { vendorId, productId }
        };
    }
}

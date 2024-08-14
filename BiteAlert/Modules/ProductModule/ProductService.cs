using BiteAlert.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Modules.ProductModule;

public class ProductService(ApplicationDbContext context, ILogger<ProductService> logger) : IProductService
{
    public async Task<UpsertProductResponse> GetProductByIdAsync(string productId)
    {
        var isValidGuid = Guid.TryParse(productId, out var productGuid);

        if (isValidGuid is false)
        {
            logger.LogWarning("Invalid product Id provided. Id: {Id}", productId);

            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Invalid product id."
            };
        }

        logger.LogInformation("Searching for product with Id {Id}", productId);

        var product = await context.Products.FindAsync(productGuid);

        if (product is null)
        {
            logger.LogWarning("Product with Id {Id} not found", productId);

            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Product not found"
            };
        }

        return new UpsertProductResponse()
        {
            Succeeded = true,
            Message = "Success",
            Product = product
        };
    }

    // Get all products
    // Note: Consider changing this to get a single vendor's products
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await context.Products.ToListAsync();
    }

    // Create a new product
    public async Task<UpsertProductResponse> CreateAsync(string vendorId, UpsertProductRequest request)
    {
        // Check if the vendorId is a valid Guid
        bool isValidGuid = Guid.TryParse(vendorId, out Guid vendorGuid);

        if (isValidGuid is false)
        {
            logger.LogWarning("Invalid vendor Id: {Id}", vendorId);

            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Invalid vendor id"
            };
        }

        var transaction = await context.Database
            .BeginTransactionAsync();

        // Get the vendor who is creating the product
        logger.LogInformation("Searching for vendor with Id {Id}", vendorGuid);

        var vendor = await context.Vendors.FindAsync(vendorGuid);

        if (vendor is null)
        {
            logger.LogWarning("vendor with Id {Id} not found", vendorGuid);

            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Vendor not found"
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

        try
        {
            // Save the product
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new UpsertProductResponse()
            {
                Succeeded = true,
                Message = "Product created successfully",
                Product = product
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred during product creation by vendor: {Id}", 
                        vendorId);

            await transaction.RollbackAsync();
            throw;
        }
    }

    // Update an existing product
    public async Task<UpsertProductResponse> UpdateAsync(string vendorId,
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

            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Vendor not found"
            };
        }

        // Return false if the vendor doesn't have any product
        if (vendor.Products is null)
        {
            logger.LogWarning("Vendor {Id} has no products.", vendorId);
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Vendor has no products"
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

            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Product not found"
            };
        }

        // Update only the fields that are not null
        logger.LogInformation("Vendor {VendorId} updating product {ProductId}", vendorId, productId);

        if (request.Name is not null) product.Name = request.Name;
        if (request.Description is not null) product.Description = request.Description;
        if (request.Price != product.Price) product.Price = request.Price;
        if (request.ImageUrl is not null) product.ImageUrl = request.ImageUrl;

        try
        {
            // Update the product
            context.Products.Update(product);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new UpsertProductResponse()
            {
                Succeeded = true,
                Message = "Product updated successfully"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while vendor {VendorId} was updating product {ProductId}",
                        vendorId,
                        productId);

            await transaction.RollbackAsync();
            throw;
        }
    }

    // Delete a vendor's product
    public async Task<UpsertProductResponse> DeleteAsync(string vendorId, string productId)
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

            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Vendor not found"
            };
        }

        // Return false if the vendor doesn't have any product
        if (vendor.Products is null)
        {
            logger.LogWarning("Vendor {Id} has no products.", vendorId);

            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Vendor has no products"
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

            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Product not found"
            };
        }

        try
        {
            // Delete the product
            logger.LogInformation("Vendor {VendorId} deleting product {ProductId}", vendorId, productId);

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new UpsertProductResponse()
            {
                Succeeded = true,
                Message = "Product deleted successfully"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while vendor {VendorId} was deleting product {ProductId}",
                        vendorId,
                        productId);

            await transaction.RollbackAsync();
            throw;
        }
    }
}

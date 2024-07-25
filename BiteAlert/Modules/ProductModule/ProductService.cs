
using BiteAlert.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Modules.ProductModule;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetProductByIdAsync(string productId)
    {
        return await _context.Products.FindAsync(productId);
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }
    public async Task<UpsertProductResponse> CreateAsync(string vendorId, UpsertProductRequest request)
    {
        var transaction = await _context.Database
            .BeginTransactionAsync();

        // Get the vendor who is creating the product
        var vendor = await _context.Vendors.FindAsync(vendorId);

        if (vendor is null)
        {
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Vendor not found"
            };
        }

        // Create a new product
        var product = new Product()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            IsAvailable = false,
            VendorId = vendorId,
            Vendor = vendor
        };

        try
        {
            // Save the product
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new UpsertProductResponse()
            {
                Succeeded = true,
                Message = "Product created successfully",
                Product = product
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<UpsertProductResponse> UpdateAsync(string vendorId, string productId, UpsertProductRequest request)
    {
        var transaction = await _context.Database
            .BeginTransactionAsync();

        // Get the vendor who is updating the product
        var vendor = await _context.Vendors.FindAsync(vendorId);

        if (vendor is null)
        {
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Vendor not found"
            };
        }

        // Get the product to be updated
        var product = await _context.Products.FindAsync(productId);

        if (product is null)
        {
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Product not found"
            };
        }

        // Update the product
        if (request.Name is not null)
        {
            product.Name = request.Name;
        }

        if (request.Description is not null)
        {
            product.Description = request.Description;
        }

        if (request.Price != product.Price)
        {
            product.Price = request.Price;
        }

        if (request.ImageUrl is not null)
        {
            product.ImageUrl = request.ImageUrl;
        }

        try
        {
            // Update the product
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new UpsertProductResponse()
            {
                Succeeded = true,
                Message = "Product updated successfully"
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string productId)
    {
        var product = await _context.Products.FindAsync(productId);

        if (product is null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}

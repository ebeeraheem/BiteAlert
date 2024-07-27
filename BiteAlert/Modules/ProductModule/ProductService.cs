﻿using BiteAlert.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Modules.ProductModule;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Get a product by it's id
    public async Task<UpsertProductResponse> GetProductByIdAsync(string productId)
    {
        var isValidGuid = Guid.TryParse(productId, out var productGuid);

        if (isValidGuid is false)
        {
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Invalid product id."
            };
        }

        var product = await _context.Products.FindAsync(productGuid);

        if (product is null)
        {
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Product not found"
            };
        }

        return new UpsertProductResponse()
        {
            Succeeded = true,
            Message = "success",
            Product = product
        };
    }

    // Get all products
    // Note: Consider changing this to get a single vendor's products
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    // Create a new product
    public async Task<UpsertProductResponse> CreateAsync(string vendorId, UpsertProductRequest request)
    {
        // Check if the vendorId is a valid Guid
        bool isValidGuid = Guid.TryParse(vendorId, out Guid vendorGuid);

        if (isValidGuid is false)
        {
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Invalid vendor id"
            };
        }

        var transaction = await _context.Database
            .BeginTransactionAsync();

        // Get the vendor who is creating the product
        var vendor = await _context.Vendors.FindAsync(vendorGuid);

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
            VendorId = vendor.Id,
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

    // Update an existing product
    public async Task<UpsertProductResponse> UpdateAsync(string vendorId, string productId, UpsertProductRequest request)
    {
        var transaction = await _context.Database
            .BeginTransactionAsync();

        // Get the vendor who is updating the product
        var vendor = await _context.Vendors
                        .Include(v => v.Products)
                        .SingleOrDefaultAsync(v => v.Id.ToString() == vendorId);

        if (vendor is null)
        {
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Vendor not found"
            };
        }

        // Return false if the vendor doesn't have any product
        if (vendor.Products is null)
        {
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
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Product not found"
            };
        }

        // Update only the fields that are not null
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

    // Delete a vendor's product
    public async Task<UpsertProductResponse> DeleteAsync(string vendorId, string productId)
    {
        var transaction = await _context.Database
            .BeginTransactionAsync();

        // Get the vendor who is deleting the product
        var vendor = await _context.Vendors
                        .Include(v => v.Products)
                        .SingleOrDefaultAsync(v => v.Id.ToString() == vendorId);

        if (vendor is null)
        {
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Vendor not found"
            };
        }

        // Return false if the vendor doesn't have any product
        if (vendor.Products is null)
        {
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
            return new UpsertProductResponse()
            {
                Succeeded = false,
                Message = "Product not found"
            };
        }

        try
        {
            // Delete the product
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new UpsertProductResponse()
            {
                Succeeded = true,
                Message = "Product deleted successfully"
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

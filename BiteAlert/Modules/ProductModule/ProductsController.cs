﻿using BiteAlert.Modules.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.ProductModule;
[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService,
                                UserContextService userContext,
                                ILogger<ProductsController> logger) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await productService.GetProductsAsync();

        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProduct(string productId)
    {
        var result = await productService.GetProductByIdAsync(productId);

        if (result.Succeeded)
        {
            logger.LogInformation("Successfully found product with Id {Id}", productId);

            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertProductRequest request)
    {
        var vendorId = userContext.GetUserId();

        if (vendorId is null)
        {
            logger.LogWarning("Unauthorized product creation attempt.");

            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            logger.LogWarning("ModelState is invalid: {ModelStateErrors}",
                        ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

            return BadRequest(ModelState);
        }

        try
        {
            logger.LogInformation("Product creation attempt by vendor with Id {Id}", vendorId);

            var result = await productService.CreateAsync(vendorId, request);

            if (result.Succeeded)
            {
                logger.LogInformation(
                    "Product created successfully by vendor {VendorId}. Product ID: {ProductId}", 
                            vendorId, 
                            result.Product!.Id);

                return CreatedAtAction(
                    nameof(GetProduct),
                    new { productId = result.Product!.Id },
                    result.Product);
            }

            logger.LogWarning("Product creation failed. Error: {Error}", result.Message);

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred during product creation by vendor: {Id}",
                        vendorId);

            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Product creation failed.");
        }
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> Update(string productId, [FromBody] UpsertProductRequest request)
    {
        var vendorId = userContext.GetUserId();

        if (vendorId is null)
        {
            logger.LogWarning("Unauthorized product update attempt.");

            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            logger.LogWarning("ModelState is invalid: {ModelStateErrors}",
                        ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

            return BadRequest(ModelState);
        }

        try
        {
            logger.LogInformation(
                "Attempting to update product {ProductId} by vendor {VendorId}", 
                        productId, 
                        vendorId);

            var result = await productService.UpdateAsync(vendorId, productId, request);

            if (result.Succeeded)
            {
                logger.LogInformation("Product {ProductId} successfully updated by vendor {VendorId}",
                            productId,
                            vendorId);

                return Ok(result);
            }

            logger.LogWarning("Failed to update product {ProductId} for vendor {VendorId}. Error: {Error}",
                        productId,
                        vendorId,
                        result.Message);

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "An unexpected error occurred while vendor {VendorId} was updating product {ProductId}",
                        vendorId,
                        productId);

            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Failed to update product.");
        }
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Delete(string productId)
    {
        var vendorId = userContext.GetUserId();

        if (vendorId is null)
        {
            logger.LogWarning("Unauthorized product deletion attempt.");

            return Unauthorized();
        }

        try
        {
            logger.LogInformation("Vendor {VendorId} attempting to delete product {ProductId}",
                        vendorId,
                        productId);

            var result = await productService.DeleteAsync(vendorId, productId);

            if (result.Succeeded)
            {
                logger.LogInformation("Product {ProductId} successfully deleted by vendor {VendorId}",
                            productId,
                            vendorId);

                return Ok(result);
            }

            logger.LogWarning("Failed to delete product {ProductId} for vendor {VendorId}. Error: {Error}",
                        productId,
                        vendorId,
                        result.Message);

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while vendor {VendorId} was deleting product {ProductId}",
                        vendorId,
                        productId);

            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Failed to delete product.");
        }
    }
}

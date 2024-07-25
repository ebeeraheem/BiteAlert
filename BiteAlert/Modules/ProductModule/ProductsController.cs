using BiteAlert.Modules.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.ProductModule;
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly UserContextService _userContext;

    public ProductsController(IProductService productService, UserContextService userContext)
    {
        _productService = productService;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.GetProductsAsync();

        return Ok(products);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProduct(string productId)
    {
        var result = await _productService.GetProductByIdAsync(productId);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertProductRequest request)
    {
        var vendorId = _userContext.GetUserId();

        if (vendorId is null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _productService.CreateAsync(vendorId, request);

            if (result.Succeeded)
            {
                return CreatedAtAction(
                    nameof(GetProduct),
                    new { productId = result.Product!.Id },
                    result.Product);
            }

            return BadRequest(result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Product creation failed.");
        }
    }
}

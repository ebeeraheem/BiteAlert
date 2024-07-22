using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.VendorModule;
[Route("api/auth")]
[ApiController]
public class VendorsController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorsController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpPost("register/vendor")]
    public async Task<IActionResult> RegisterVendor([FromBody] VendorRegistrationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _vendorService.RegisterVendorAsync(request);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Registration successful");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occured.");
        }
    }
}

using BiteAlert.Modules.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.VendorModule;
[Route("api/vendor")]
[ApiController]
public class VendorsController : ControllerBase
{
    private readonly IVendorService _vendorService;
    private readonly UserContextService _userContext;

    public VendorsController(IVendorService vendorService, UserContextService userContext)
    {
        _vendorService = vendorService;
        _userContext = userContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterVendor([FromBody] RegisterVendorRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = _userContext.GetUserId();

            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _vendorService
                .RegisterVendorAsync(userId, request);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occured.");
        }
    }
}

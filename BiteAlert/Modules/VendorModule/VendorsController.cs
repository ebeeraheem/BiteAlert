using BiteAlert.Modules.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.VendorModule;
[Route("api/[controller]")]
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
    public async Task<IActionResult> RegisterVendor([FromBody] UpsertVendorRequest request)
{
        var userId = _userContext.GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _vendorService
                .RegisterVendorAsync(userId, request);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    [HttpGet("by-id/{vendorId}")]
    public async Task<IActionResult> GetVendorById(string vendorId)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

        if (vendor is null)
        {
            return NotFound("vendor not found");
        }

        return Ok(vendor);
    }

    [HttpGet("by-username/{userName}")]
    public async Task<IActionResult> GetVendorByUserName(string userName)
    {
        var vendor = await _vendorService.GetVendorByUserNameAsync(userName);

        if (vendor is null)
        {
            return NotFound("vendor not found");
        }

        return Ok(vendor);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateBusinessInfo([FromBody] UpsertVendorRequest request)
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
            var result = await _vendorService
                                    .UpdateVendorBusinessInfo(vendorId, request);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }
}

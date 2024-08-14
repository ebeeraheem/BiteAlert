using BiteAlert.Modules.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.VendorModule;
[Route("api/[controller]")]
[ApiController]
public class VendorsController(IVendorService vendorService,
                               UserContextService userContext,
                               ILogger<VendorsController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterVendor([FromBody] UpsertVendorRequest request)
{
        var userId = userContext.GetUserId();

        if (userId is null)
        {
            logger.LogWarning("Unauthorized vendor registration attempt.");

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
            logger.LogInformation("Attempting to register vendor with Id {Id}", userId);

            var result = await vendorService
                        .RegisterVendorAsync(userId, request);

            if (result.Succeeded)
            {
                logger.LogInformation("User {Id} successfully registered as a vendor.", userId);

                return Ok(result);
            }

            logger.LogWarning("Vendor registration failed. User ID: {Id}. Error: {Error}",
                        userId,
                        result.Message);

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred during vendor registration. ID: {Id}",
                        userId);

            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An unexpected error occurred. Vendor registration failed.");
        }
    }

    [HttpGet("by-id/{vendorId}")]
    public async Task<IActionResult> GetVendorById(string vendorId)
    {
        logger.LogInformation("Attempting to get vendor by Id {Id}", vendorId);

        var vendor = await vendorService.GetVendorByIdAsync(vendorId);

        if (vendor is null)
        {
            logger.LogWarning("Vendor with Id {Id} not found.", vendorId);

            return NotFound("vendor not found");
        }

        return Ok(vendor);
    }

    [HttpGet("by-username/{userName}")]
    public async Task<IActionResult> GetVendorByUserName(string userName)
    {
        logger.LogInformation("Attempting to find vendor with username {Username}", userName);

        var vendor = await vendorService.GetVendorByUserNameAsync(userName);

        if (vendor is null)
        {
            logger.LogWarning("Vendor with username {Username} not found.", userName);

            return NotFound("vendor not found");
        }

        return Ok(vendor);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateBusinessInfo([FromBody] UpsertVendorRequest request)
    {
        var vendorId = userContext.GetUserId();

        if (vendorId is null)
        {
            logger.LogWarning("Unauthorized attempt to update vendor business information.");

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
            logger.LogInformation("Attempting to update business information for vendor {VendorId}", vendorId);

            var result = await vendorService
                                    .UpdateVendorBusinessInfo(vendorId, request);

            if (result.Succeeded)
            {
                logger.LogInformation("Successfully updated business information for vendor {Id}", vendorId);

                return Ok(result);
            }

            logger.LogWarning("Failed to update business information for vendor {Id}. Error: {Error}",
                        vendorId,
                        result.Message);

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while updating business information for vendor {VendorId}",
                        vendorId);

            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }
}

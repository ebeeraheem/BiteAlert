using Asp.Versioning;
using BiteAlert.Modules.Shared;
using BiteAlert.Modules.Utilities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.VendorModule.V1;
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/vendors")]
public class VendorsController(IVendorService vendorService,
                               UserContextService userContext,
                               ILogger<VendorsController> logger,
                               IValidator<UpsertVendorRequest> validator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterVendor([FromBody] UpsertVendorRequest request)
    {
        var userId = userContext.GetUserId();
        if (userId is null)
        {
            logger.LogWarning("Unauthorized vendor registration attempt.");
            return Unauthorized();
        }

        var validationResult = validator.Validate(request);
        if (validationResult.IsValid is false)
        {
            var failedResponse = new UpsertVendorResponse
            {
                Succeeded = false,
                Message = "Failed to register vendor.",
                FluentValidationErrors = validationResult.Errors
                    .Select(error => new FluentValidationError()
                    {
                        PropertyName = error.PropertyName,
                        ErrorMessage = error.ErrorMessage
                    })
            };
            logger.LogWarning("Register vendor request failed validation. Errors: {Errors}",
                        validationResult);

            return BadRequest(failedResponse);
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVendorById(string vendorId)
    {
        logger.LogInformation("Attempting to get vendor by Id {Id}", vendorId);

        var vendor = await vendorService.GetVendorByIdAsync(vendorId);

        if (vendor is null)
        {
            logger.LogWarning("Vendor with Id {Id} not found.", vendorId);

            return NotFound("Vendor not found");
        }

        return Ok(vendor);
    }

    [HttpGet("by-username/{userName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVendorsByUserName(string userName)
    {
        logger.LogInformation("Attempting to find vendors with username {Username}", userName);

        var vendor = await vendorService.GetVendorsByUserNameAsync(userName);

        if (vendor is null)
        {
            logger.LogWarning("Vendors with username {Username} not found.", userName);

            return NotFound("No vendor found.");
        }

        return Ok(vendor);
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateBusinessInfo([FromBody] UpsertVendorRequest request)
    {
        var vendorId = userContext.GetUserId();

        if (vendorId is null)
        {
            logger.LogWarning("Unauthorized attempt to update vendor business information.");

            return Unauthorized();
        }

        var validationResult = validator.Validate(request);

        if (validationResult.IsValid is false)
        {
            logger.LogWarning("Update vendor request failed validation. Errors: {Errors}",
                        validationResult);

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

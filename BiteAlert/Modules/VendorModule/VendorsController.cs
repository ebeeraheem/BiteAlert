﻿using BiteAlert.Modules.Utilities;
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
}

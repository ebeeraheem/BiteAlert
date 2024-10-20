﻿using Asp.Versioning;
using BiteAlert.Modules.Utilities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.UserModule.V1;
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(IUserService userService,
                            UserContextService userContext,
                            ILogger<UsersController> logger,
                            IValidator<UserProfileRequest> profileValidator) : ControllerBase
{
    [HttpPost("update/role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SelectRole(string roleName)
    {
        var userId = userContext.GetUserId();
        if (userId is null)
        {
            logger.LogWarning("Unauthorized role selection attempt.");
            return Unauthorized();
        }

        var result = await userService.SelectRoleAsync(userId, roleName);
        if (result.Succeeded)
        {
            logger.LogInformation("Role assigned successfully. User ID: {Id}", userId);
            return Ok(result);
        }

        logger.LogWarning("Failed to assign role. User ID: {Id}", userId);
        return BadRequest(result);
    }

    [HttpPut("update/profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileRequest request)
    {
        var userId = userContext.GetUserId();

        if (userId is null)
        {
            logger.LogWarning("Unauthorized profile update attempt.");

            return Unauthorized();
        }

        var validationResult = profileValidator.Validate(request);

        if (validationResult.IsValid is false)
        {
            var failedResponse = new UserProfileResponse()
            {
                Succeeded = false,
                Message = "Failed to update profile.",
                Data = new { validationResult.Errors }
            };

            logger.LogWarning("User profile request failed validation. Errors: {Errors}",
                        validationResult);

            return BadRequest(failedResponse);
        }

        try
        {
            logger.LogInformation("Attempting to update profile for user with Id: {Id}", userId);

            var result = await userService.UpdateProfileAsync(userId, request);

            if (result.Succeeded)
            {
                logger.LogInformation("Successfully updated profile for user with Id: {Id}", userId);

                return Ok(result);
            }

            logger.LogWarning("Failed to update profile for user with Id: {Id}. Error: {Error}",
                        userId,
                        result.Message);

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "An unexpected error occurred while updating profile for user with Id: {Id}",
                        userId);

            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Failed to update user profile.");
        }
    }
}

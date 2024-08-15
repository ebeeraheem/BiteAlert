// Ignore Spelling: Auth

using BiteAlert.Modules.Utilities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.Authentication;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserService userService,
                            UserContextService userContext,
                            ILogger<AuthController> logger,
                            IValidator<RegisterUserRequest> registerValidator,
                            IValidator<LoginUserRequest> loginValidator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var validationResult = registerValidator.Validate(request);

        if (validationResult.IsValid is false)
        {
            logger.LogWarning("Register user request failed validation. Errors: {Errors}",
                        validationResult);

            return BadRequest(new { 
                Succeeded = false, 
                Message = "User registration failed.", 
                // Errors = Select only the Property name and error message fields
            });
        }

        try
        {
            var result = await userService.RegisterUserAsync(request);

            if (result.Errors is null)
            {
                logger.LogInformation("User successfully registered with email: {Email}", request.Email);

                return Ok(result);
            }

            logger.LogWarning("User registration failed with email: {Email}. Errors: {@Errors}", 
                        request.Email, 
                        result.Errors);

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred during user registration with email: {Email}", 
                        request.Email);

            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An unexpected error occurred. User registration failed.");
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        var validationResult = loginValidator.Validate(request);

        if (validationResult.IsValid is false)
        {
            logger.LogWarning("Login user request failed validation. Errors: {Errors}",
                        validationResult);

            return BadRequest(validationResult.Errors);
        }

        try
        {
            logger.LogInformation("Attempting login for user with email: {Email}", request.Email);

            var result = await userService.LoginUserAsync(request);

            if (result.Token is null)
            {
                logger.LogWarning("Failed to login user with email: {Email}. Error: {Error}",
                            request.Email,
                            result.Message);

                return BadRequest(result);
            }

            logger.LogInformation("Successfully login user with email: {Email}", request.Email);

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred during user login with email: {Email}",
                        request.Email);

            return StatusCode(StatusCodes.Status500InternalServerError, 
                "An unexpected error occurred. User login failed.");
        }
    }

    [HttpPost("update/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileRequest request)
    {
        var userId = userContext.GetUserId();

        if (userId is null)
        {
            logger.LogWarning("Unauthorized profile update attempt.");

            return Unauthorized();
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

    [HttpPost("update/password")]
    public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordRequest request)
    {
        var userId = userContext.GetUserId();

        if (userId is null)
        {
            logger.LogWarning("Unauthorized password change attempt.");

            return Unauthorized();
        }

        logger.LogInformation("Attempting to update password for user with Id {Id}", userId);

        if (request.CurrentPassword == request.NewPassword)
        {
            logger.LogWarning("New password cannot be the same as old password. User ID: {Id}",
                        userId);

            return BadRequest("New password cannot be the same as old password");
        }

        var result = await userService.UpdatePasswordAsync(userId, request);

        if (result.Succeeded)
        {
            logger.LogInformation("Successfully updated password for user with Id {Id}", userId);

            return Ok(result);
        }

        logger.LogWarning("Failed to update password for user with Id {Id}. Errors: {@Errors}", 
                    userId,
                    result.Errors);

        return BadRequest(result);
    }
}

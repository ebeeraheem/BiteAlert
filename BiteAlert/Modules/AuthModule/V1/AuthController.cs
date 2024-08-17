// Ignore Spelling: Auth

using Asp.Versioning;
using BiteAlert.Modules.Shared;
using BiteAlert.Modules.Utilities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.AuthModule.V1;
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(IAuthService userService,
                            UserContextService userContext,
                            ILogger<AuthController> logger,
                            IValidator<RegisterUserRequest> registerValidator,
                            IValidator<LoginUserRequest> loginValidator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var validationResult = registerValidator.Validate(request);

        if (validationResult.IsValid is false)
        {
            var failedResponse = new RegisterUserResponse
            {
                Succeeded = false,
                Message = "User registration failed.",
                FluentValidationErrors = validationResult.Errors
                    .Select(error => new FluentValidationError
                    {
                        PropertyName = error.PropertyName,
                        ErrorMessage = error.ErrorMessage
                    })
            };

            logger.LogWarning("Register user request failed validation. Errors: {Errors}",
                        validationResult);

            return BadRequest(failedResponse);
        }

        try
        {
            var result = await userService.RegisterUserAsync(request);

            if (result.IdentityErrors is null)
            {
                logger.LogInformation("User successfully registered with email: {Email}", request.Email);

                return Ok(result);
            }

            logger.LogWarning("User registration failed with email: {Email}. Errors: {@Errors}",
                        request.Email,
                        result.IdentityErrors);

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        var validationResult = loginValidator.Validate(request);

        if (validationResult.IsValid is false)
        {
            var failedResponse = new LoginUserResponse
            {
                Succeeded = false,
                Message = "User login failed.",
                FluentValidationErrors = validationResult.Errors
                    .Select(error => new FluentValidationError
                    {
                        PropertyName = error.PropertyName,
                        ErrorMessage = error.ErrorMessage
                    })
            };

            logger.LogWarning("Login user request failed validation. Errors: {Errors}",
                        validationResult);

            return BadRequest(failedResponse);
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

    [AllowAnonymous]
    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        try
        {
            logger.LogInformation("Attempting to verify email address. Email: {Email}",
                        request.Email);

            var result = await userService.VerifyEmailAsync(request);

            if (result.Succeeded)
            {
                logger.LogInformation("Email confirmed successfully. Email: {Email}",
                            request.Email);

                return Ok(result);
            }

            logger.LogWarning("Email confirmation failed. Email: {Email}", request.Email);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred during email confirmation. Email: {Email}",
                        request.Email);

            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Email confirmation failed.");
        }
    }

    [HttpPost("update/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

            return BadRequest("New password cannot be the same as old password.");
        }

        var result = await userService.UpdatePasswordAsync(userId, request);

        if (result.Succeeded)
        {
            logger.LogInformation("Successfully updated password for user with Id {Id}", userId);

            return Ok(result);
        }

        logger.LogWarning("Failed to update password for user with Id {Id}. Errors: {@Errors}",
                    userId,
                    result.IdentityErrors);

        return BadRequest(result);
    }
}

// Ignore Spelling: Auth

using BiteAlert.Modules.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.Authentication;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly UserContextService _userContext;

    public AuthController(IUserService userService, UserContextService userContext)
    {
        _userService = userService;
        _userContext = userContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _userService.RegisterUserAsync(request);

            if (result.Errors is null)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. User registration failed.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _userService.LoginUserAsync(request);

            if (result.Token is null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. User login failed.");
        }
    }

    [HttpPost("update/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileRequest request)
    {
        var userId = _userContext.GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _userService.UpdateProfileAsync(userId, request);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Failed to update user profile.");
        }
    }

    [HttpPost("update/password")]
    public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordRequest request)
    {
        var userId = _userContext.GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        if (request.CurrentPassword == request.NewPassword)
        {
            return BadRequest("New password cannot be the same as old password");
        }

        var result = await _userService.UpdatePasswordAsync(userId, request);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}

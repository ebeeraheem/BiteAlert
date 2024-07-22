using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.Authentication;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
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

            if (result.Error is null)
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
}

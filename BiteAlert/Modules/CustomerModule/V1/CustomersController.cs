using Asp.Versioning;
using BiteAlert.Modules.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.CustomerModule.V1;
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customers")]
public class CustomersController(ICustomerService customerService,
                                 UserContextService userContext,
                                 ILogger<CustomersController> logger) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RegisterCustomer()
    {
        var userId = userContext.GetUserId();

        if (userId is null)
        {
            logger.LogWarning("Unauthorized customer registration attempt.");

            return Unauthorized();
        }

        logger.LogInformation("Attempting to register customer with Id {Id}", userId);

        var result = await customerService.RegisterCustomerAsync(userId);

        if (result.Succeeded)
        {
            logger.LogInformation("User {Id} successfully registered as a customer", userId);

            return Ok(result);
        }

        logger.LogWarning("Customer registration failed. User ID: {Id}. Error: {Error}",
                    userId,
                    result.Message);

        return BadRequest(result);
    }

    [HttpGet("by-id/{customerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerById(string customerId)
    {
        var customer = await customerService.GetCustomerById(customerId);

        if (customer is null)
        {
            logger.LogWarning("Customer with Id {Id} not found", customerId);

            return NotFound("Customer not found");
        }

        return Ok(customer);
    }
}

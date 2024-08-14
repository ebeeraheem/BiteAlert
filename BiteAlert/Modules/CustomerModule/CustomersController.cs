using BiteAlert.Modules.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.CustomerModule;
[Route("api/[controller]")]
[ApiController]
public class CustomersController(ICustomerService customerService,
                                 UserContextService userContext,
                                 ILogger<CustomersController> logger): ControllerBase
{
    [HttpPost("register")]
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

using BiteAlert.Modules.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BiteAlert.Modules.CustomerModule;
[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly UserContextService _userContext;

    public CustomersController(ICustomerService customerService, UserContextService userContext)
    {
        _customerService = customerService;
        _userContext = userContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterCustomer()
    {
        var userId = _userContext.GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await _customerService
                                        .RegisterCustomerAsync(userId);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("by-id/{customerId}")]
    public async Task<IActionResult> GetCustomerById(string customerId)
    {
        var customer = await _customerService.GetCustomerById(customerId);

        if (customer is null)
        {
            return NotFound("Customer not found");
        }

        return Ok(customer);
    }
}

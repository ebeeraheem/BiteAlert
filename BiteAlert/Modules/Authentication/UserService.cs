using BiteAlert.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.Authentication;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Register a new application user
    public async Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request)
    {
        var transaction = await _context.Database
            .BeginTransactionAsync();

        try
        {
            var user = new ApplicationUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var failedResponse = new RegisterUserResponse()
                {
                    Status = "failed",
                    Message = "user registration failed",
                    Error = result.Errors
                };

                return failedResponse;
            }

            var successResponse = new RegisterUserResponse()
            {
                Status = "success",
                Message = "user registered successfully"
            };

            await transaction.CommitAsync();
            return successResponse;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

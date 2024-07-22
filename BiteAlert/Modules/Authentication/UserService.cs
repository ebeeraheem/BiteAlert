using BiteAlert.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BiteAlert.Modules.Authentication;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public UserService(ApplicationDbContext context,
                        UserManager<ApplicationUser> userManager,
                        SignInManager<ApplicationUser> signInManager,
                        IConfiguration config)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
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
                    Succeeded = false,
                    Message = "user registration failed",
                    Error = result.Errors
                };

                return failedResponse;
            }

            var successResponse = new RegisterUserResponse()
            {
                Succeeded = true,
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

    // Login user
    public async Task<LoginUserResponse> LoginUserAsync(LoginUserRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return new LoginUserResponse()
            {
                Succeeded = false,
                Message = "user not found"
            };
        }

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

        if (!result.Succeeded)
        {
            return new LoginUserResponse()
            {
                Succeeded = false,
                Message = "invalid credentials"
            };
        }

        string tokenString = GenerateJwtToken(user);

        var response = new LoginUserResponse()
        {
            Succeeded = true,
            Message = "user logged in successfully",
            Token = tokenString
        };

        return response;
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        // Get configuration values
        var key = _config.GetValue<string>("Jwt:Key");
        var issuer = _config.GetValue<string>("Jwt:Issuer");
        var audience = _config.GetValue<string>("Jwt:Audience");

        // Ensure configuration values are not null or empty
        if (string.IsNullOrEmpty(key) || 
            string.IsNullOrEmpty(issuer) || 
            string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("JWT configuration values are missing.");
        }

        var claims = new List<Claim>()
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        // Generate token
        var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: DateTime.UtcNow.AddMinutes(3),
        signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
}

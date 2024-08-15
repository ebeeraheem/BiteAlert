// Ignore Spelling: Auth

using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BiteAlert.Modules.AuthModule;

public class AuthService(ApplicationDbContext context,
                         UserManager<ApplicationUser> userManager,
                         SignInManager<ApplicationUser> signInManager,
                         IConfiguration config,
                         ILogger<AuthService> logger) : IAuthService
{
    public async Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request)
    {
        var transaction = await context.Database
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

            logger.LogInformation("Attempting to register user with email: {Email}", request.Email);

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded is false)
            {
                var failedResponse = new RegisterUserResponse()
                {
                    Succeeded = false,
                    Message = "User registration failed.",
                    IdentityErrors = result.Errors
                };

                await transaction.RollbackAsync();
                return failedResponse;
            }

            var successResponse = new RegisterUserResponse()
            {
                Succeeded = true,
                Message = "User registered successfully."
            };

            await transaction.CommitAsync();
            return successResponse;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred during user registration with email: {Email}",
                        request.Email);

            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<LoginUserResponse> LoginUserAsync(LoginUserRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found", request.Email);

            return new LoginUserResponse()
            {
                Succeeded = false,
                Message = "User not found"
            };
        }

        logger.LogInformation("Attempting password sign in for user with email {Email}", user.Email);

        var result = await signInManager.PasswordSignInAsync(user,
                                                             request.Password,
                                                             false,
                                                             false);

        if (result.Succeeded is false)
        {
            logger.LogWarning("Invalid login credentials for email {Email}", user.Email);

            return new LoginUserResponse()
            {
                Succeeded = false,
                Message = "Invalid credentials"
            };
        }

        logger.LogInformation("User successfully logged in with email: {Email}.", user.Email);

        string tokenString = GenerateJwtToken(user);

        logger.LogInformation("Successfully generated JWT token for user with email: {Email}.", user.Email);

        var response = new LoginUserResponse()
        {
            Succeeded = true,
            Message = "User logged in successfully",
            Token = tokenString
        };

        return response;
    }

    public async Task<AuthResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found", userId);

            return new AuthResponse()
            {
                Succeeded = false,
                Message = "User not found"
            };
        }

        logger.LogInformation("Updating password for user with Id: {Id}", user.Id);

        var result = await userManager.ChangePasswordAsync(user,
                                                           request.CurrentPassword,
                                                           request.NewPassword);

        if (result.Succeeded is false)
        {
            return new AuthResponse()
            {
                Succeeded = false,
                Message = "Failed to update password.",
                IdentityErrors = result.Errors
            };
        }

        return new AuthResponse()
        {
            Succeeded = true,
             Message = "Password updated successfully."
        };
    }

    public Task<AuthResponse> ResetPasswordAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> VerifyEmailAsync()
    {
        throw new NotImplementedException();
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        logger.LogInformation("Generating JWT token for user with email: {Email}", user.Email);

        // Get configuration values
        var key = config.GetValue<string>("Jwt:Key");
        var issuer = config.GetValue<string>("Jwt:Issuer");
        var audience = config.GetValue<string>("Jwt:Audience");

        // Ensure configuration values are not null or empty
        if (string.IsNullOrEmpty(key) || 
            string.IsNullOrEmpty(issuer) || 
            string.IsNullOrEmpty(audience))
        {
            logger.LogWarning("Failed to get JWT configuration values for user with email: {Email}",
                        user.UserName);

            throw new InvalidOperationException("JWT configuration values are missing.");
        }

        var claims = new List<Claim>()
        {
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
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

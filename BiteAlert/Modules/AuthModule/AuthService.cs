// Ignore Spelling: Auth

using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.NotificationModule;
using BiteAlert.Modules.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace BiteAlert.Modules.AuthModule;

public class AuthService(ApplicationDbContext context,
                         UserManager<ApplicationUser> userManager,
                         SignInManager<ApplicationUser> signInManager,
                         IMediator mediator,
                         IConfiguration config,
                         ILogger<AuthService> logger) : IAuthService
{
    public async Task<Shared.BaseResponse> RegisterUserAsync(RegisterUserRequest request)
    {
        var transaction = await context.Database
            .BeginTransactionAsync();

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
            var failedResponse = new BaseResponse()
            {
                Succeeded = false,
                Message = "User registration failed.",
                Data = result.Errors
            };

            await transaction.RollbackAsync();
            return failedResponse;
        }

        var emailConfirmationToken = await userManager
            .GenerateEmailConfirmationTokenAsync(user);

        // Publish user registered event
        logger.LogInformation("Publishing user registered event.");
        await mediator.Publish(new UserRegisteredEvent
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmationToken = emailConfirmationToken
        });

        // Login and generate token
        await signInManager.PasswordSignInAsync(user, request.Password, false, false);

        string tokenString = GenerateJwtToken(user);

        var successResponse = new BaseResponse()
        {
            Succeeded = true,
            Message = "User registered successfully.",
            Data = new { tokenString }
        };

        await transaction.CommitAsync();
        return successResponse;
    }

    public async Task<BaseResponse> LoginUserAsync(LoginUserRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found.", request.Email);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User not found.",
                Data = new { request.Email }
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

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Invalid credentials.",
                Data = new { request.Email }
            };
        }

        logger.LogInformation("User successfully logged in with email: {Email}", user.Email);

        string tokenString = GenerateJwtToken(user);

        logger.LogInformation("Successfully generated JWT token for user with email: {Email}", user.Email);

        var response = new BaseResponse()
        {
            Succeeded = true,
            Message = "User logged in successfully.",
            Data = new { tokenString }
        };

        return response;
    }

    public async Task<BaseResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found", userId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User not found.",
                Data = new { userId }
            };
        }

        logger.LogInformation("Updating password for user with Id: {Id}", user.Id);

        var result = await userManager.ChangePasswordAsync(user,
                                                           request.CurrentPassword,
                                                           request.NewPassword);

        if (result.Succeeded is false)
        {
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Failed to update password.",
                Data = new { result.Errors }
            };
        }

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Password updated successfully.",
            Data = new { user.Id }
        };
    }

    public async Task<BaseResponse> SendPasswordResetEmail(ForgotPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            logger.LogWarning("User not found. Email: {Email}", request.Email);
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User not found.",
                Data = new { request.Email }
            };
        }

        var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        await mediator.Publish(new PasswordResetEvent
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PasswordResetToken = passwordResetToken
        });

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Password reset token sent successfully.",
            Data = new { user.Id }
        };
    }

    public async Task<BaseResponse> ResetPasswordAsync(PasswordResetRequest request)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            logger.LogWarning("User not found. User ID: {Id}", request.UserId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User not found.",
                Data = new { request.UserId }
            };
        }

        var decodedToken = HttpUtility.UrlDecode(request.PasswordResetToken);

        var result = await userManager.ResetPasswordAsync(user,
                                                          decodedToken,
                                                          request.ConfirmPassword);

        if (result.Succeeded is false)
        {
            return new BaseResponse
            {
                Succeeded = false,
                Message = "Password reset failed.",
                Data = new { result.Errors }
            };
        }

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Password reset successful.",
            Data = new { user.Id }
        };
    }

    public async Task<BaseResponse> VerifyEmailAsync(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogWarning("User not found. User ID: {Id}", userId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User not found.",
                Data = new { userId }
            };
        }

        var result = await userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded is false)
        {
            return new BaseResponse
            {
                Succeeded = false,
                Message = "Email confirmation failed.",
                Data = new { result.Errors }
            };
        }

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Email confirmed successfully.",
            Data = new { userId }
        };
    }

    public async Task<BaseResponse> SendVerificationEmailAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found", userId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User not found.",
                Data = new { userId }
            };
        }

        if (await userManager.IsEmailConfirmedAsync(user))
        {
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Email is already confirmed.",
                Data = new { userId }
            };
        }

        var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

        await mediator.Publish(new UserRegisteredEvent
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmationToken = emailConfirmationToken
        });

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Confirmation email sent successfully.",
            Data = new { userId }
        };
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
            logger.LogError("Failed to get JWT configuration values for user with email: {Email}",
                        user.UserName);

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
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
}

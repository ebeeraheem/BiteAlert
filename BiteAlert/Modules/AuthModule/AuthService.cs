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

namespace BiteAlert.Modules.AuthModule;

public class AuthService(ApplicationDbContext context,
                         UserManager<ApplicationUser> userManager,
                         SignInManager<ApplicationUser> signInManager,
                         IMediator mediator,
                         IConfiguration config,
                         ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponse> RegisterUserAsync(RegisterUserRequest request)
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
                var failedResponse = new AuthResponse()
                {
                    Succeeded = false,
                    Message = "User registration failed.",
                    IdentityErrors = result.Errors
                };

                await transaction.RollbackAsync();
                return failedResponse;
            }

            // Generate email verification token
            var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

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

            var successResponse = new AuthResponse()
            {
                Succeeded = true,
                Message = "User registered successfully.",
                Token = tokenString
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

    public async Task<AuthResponse> LoginUserAsync(LoginUserRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found.", request.Email);

            return new AuthResponse()
            {
                Succeeded = false,
                Message = "User not found."
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

            return new AuthResponse()
            {
                Succeeded = false,
                Message = "Invalid credentials."
            };
        }

        logger.LogInformation("User successfully logged in with email: {Email}", user.Email);

        string tokenString = GenerateJwtToken(user);

        logger.LogInformation("Successfully generated JWT token for user with email: {Email}", user.Email);

        var response = new AuthResponse()
        {
            Succeeded = true,
            Message = "User logged in successfully.",
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
                Message = "User not found."
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

    public async Task<AuthResponse> SendPasswordResetEmail(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found", userId);
            return new AuthResponse()
            {
                Succeeded = false,
                Message = "User not found."
            };
        }

        var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        await mediator.Publish(new PasswordResetEvent
        {
            UserId = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            PasswordResetToken = passwordResetToken
        });

        return new AuthResponse()
        {
            Succeeded = true,
            Message = "Password reset token sent successfully."
        };
    }

    public async Task<AuthResponse> ResetPasswordAsync(PasswordResetRequest model)
    {
        var user = await userManager.FindByIdAsync(model.UserId);
        if (user is null)
        {
            logger.LogWarning("User not found. User ID: {Id}", model.UserId);

            return new AuthResponse()
            {
                Succeeded = false,
                Message = "User not found."
            };
        }

        var result = await userManager.ResetPasswordAsync(user,
                                                          model.PasswordResetToken,
                                                          model.ConfirmPassword);

        if (result.Succeeded is false)
        {
            return new AuthResponse
            {
                Succeeded = false,
                Message = "Password reset failed.",
                IdentityErrors = result.Errors
            };
        }

        return new AuthResponse()
        {
            Succeeded = true,
            Message = "Password reset successful."
        };
    }

    public async Task<AuthResponse> VerifyEmailAsync(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogWarning("User not found. User ID: {Id}", userId);

            return new AuthResponse()
            {
                Succeeded = false,
                Message = "User not found."
            };
        }

        var result = await userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded is false)
        {
            return new AuthResponse
            {
                Succeeded = false,
                Message = "Email confirmation failed.",
                IdentityErrors = result.Errors
            };
        }

        return new AuthResponse()
        {
            Succeeded = true,
            Message = "Email confirmed successfully."
        };
    }

    public async Task<AuthResponse> SendVerificationEmailAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found", userId);

            return new AuthResponse()
            {
                Succeeded = false,
                Message = "User not found."
            };
        }

        if (await userManager.IsEmailConfirmedAsync(user))
        {
            return new AuthResponse()
            {
                Succeeded = false,
                Message = "Email is already confirmed."
            };
        }

        var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

        await mediator.Publish(new UserRegisteredEvent
        {
            UserId = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            EmailConfirmationToken = emailConfirmationToken
        });

        return new AuthResponse()
        {
            Succeeded = true,
            Message = "Confirmation email sent successfully."
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

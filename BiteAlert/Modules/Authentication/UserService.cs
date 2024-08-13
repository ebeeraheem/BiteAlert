using BiteAlert.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BiteAlert.Modules.Authentication;

public class UserService(ApplicationDbContext context,
                         UserManager<ApplicationUser> userManager,
                         SignInManager<ApplicationUser> signInManager,
                         IConfiguration config,
                         ILogger<UserService> logger) : IUserService
{

    // Register a new application user
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

            logger.LogInformation("Creating user with email: {Email}", request.Email);

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded is false)
            {
                var failedResponse = new RegisterUserResponse()
                {
                    Succeeded = false,
                    Message = "User registration failed.",
                    Errors = result.Errors
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

    // Login user
    public async Task<LoginUserResponse> LoginUserAsync(LoginUserRequest request)
    {
        logger.LogInformation("Searching for user with email: {Email}", request.Email);

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

        logger.LogInformation("User {Username} logged in successfully.", user.UserName);

        string tokenString = GenerateJwtToken(user);

        logger.LogInformation("Successfully generated JWT token for user {Username}", user.UserName);

        var response = new LoginUserResponse()
        {
            Succeeded = true,
            Message = "User logged in successfully",
            Token = tokenString
        };

        return response;
    }

    // Update user profile info
    public async Task<UserProfileResponse> UpdateProfileAsync(string userId, UserProfileRequest request)
    {
        var transaction = await context.Database
            .BeginTransactionAsync();

        try
        {
            logger.LogInformation("Searching for user by Id: {Id}", userId);

            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                logger.LogWarning("User with Id {Id} not found", userId);

                return new UserProfileResponse()
                {
                    Succeeded = false,
                    Message = "User not found"
                };
            }

            logger.LogInformation("Updating profile information for user: {Username}", user.UserName);

            if (request.FirstName is not null)
            {
                user.FirstName = request.FirstName;
            }

            if (request.LastName is not null)
            {
                user.LastName = request.LastName;
            }

            if (request.UserName is not null)
            {
                user.UserName = request.UserName;
            }

            if (request.PhoneNumber is not null)
            {
                user.PhoneNumber = request.PhoneNumber;
            }

            if (request.DateOfBirth is not null)
            {
                user.DateOfBirth = request.DateOfBirth;
            }

            if (request.ProfilePictureUrl is not null)
            {
                user.ProfilePictureUrl = request.ProfilePictureUrl;
            }

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded is false)
            {
                logger.LogWarning("Failed to update profile information for {Username}. Errors: {@Errors}", 
                            user.UserName,
                            result.Errors);

                return new UserProfileResponse()
                {
                    Succeeded = false,
                    Message = "Failed to update user profile.",
                    Errors = result.Errors
                };
            }

            logger.LogInformation("User {Username} successfully updated their profile information.",
                        user.UserName);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new UserProfileResponse()
            {
                Succeeded = true,
                Message = "User profile updated successfully."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating profile information for user with Id: {Id}", 
                        userId);

            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<UserProfileResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request)
    {
        logger.LogInformation("Searching for user with Id: {Id}", userId);

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found", userId);

            return new UserProfileResponse()
            {
                Succeeded = false,
                Message = "User not found"
            };
        }

        logger.LogInformation("Updating password for user {Username}", user.UserName);

        var result = await userManager.ChangePasswordAsync(user,
                                                           request.CurrentPassword,
                                                           request.NewPassword);

        if (result.Succeeded is false)
        {
            return new UserProfileResponse()
            {
                Succeeded = false,
                Message = "Failed to change password",
                Errors = result.Errors
            };
        }

        return new UserProfileResponse()
        {
            Succeeded = true,
             Message = "Password updated successfully"
        };
    }

    public Task<UserProfileResponse> ResetPasswordAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileResponse> VerifyEmailAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileResponse> UpdateEmailAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileResponse> DeleteUserAccount()
    {
        throw new NotImplementedException();
    }
    private string GenerateJwtToken(ApplicationUser user)
    {
        logger.LogInformation("Generating JWT token for user {Username}", user.UserName);

        // Get configuration values
        var key = config.GetValue<string>("Jwt:Key");
        var issuer = config.GetValue<string>("Jwt:Issuer");
        var audience = config.GetValue<string>("Jwt:Audience");

        // Ensure configuration values are not null or empty
        if (string.IsNullOrEmpty(key) || 
            string.IsNullOrEmpty(issuer) || 
            string.IsNullOrEmpty(audience))
        {
            logger.LogWarning("Failed to get JWT configuration values for user {Username}", user.UserName);

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

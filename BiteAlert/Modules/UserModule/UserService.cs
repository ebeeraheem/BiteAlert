using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Shared;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace BiteAlert.Modules.UserModule;

public class UserService(ApplicationDbContext context,
                         UserManager<ApplicationUser> userManager,
                         RoleManager<IdentityRole<Guid>> roleManager,
                         ILogger<UserService> logger) : IUserService

{
    public async Task<UserProfileResponse> SelectRoleAsync(string userId, string roleName)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found.", userId);

            return new UserProfileResponse()
            {
                Succeeded = false,
                Message = "User not found."
            };
        }

        if (await roleManager.RoleExistsAsync(roleName) is false)
        {
            return new UserProfileResponse()
            {
                Succeeded = false,
                Message = "Selected role does not exist."
            };
        }

        var userRoles = await userManager.GetRolesAsync(user);

        if (userRoles.Any() && userRoles.Contains("Admin") is false)
        {
            return new UserProfileResponse()
            {
                Succeeded = false,
                Message = "Role selection is not allowed."
            };
        }

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded is false)
        {
            return new UserProfileResponse()
            {
                Succeeded = false,
                Message = "Failed to assign role."
            };
        }

        user.LastUpdatedAt = DateTime.UtcNow;
        return new UserProfileResponse()
        {
            Succeeded = true,
            Message = "Role assigned successfully."
        };
    }
    public async Task<UserProfileResponse> UpdateProfileAsync(string userId, UserProfileRequest request)
    {
        var transaction = await context.Database
        .BeginTransactionAsync();

        try
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                logger.LogWarning("User with Id {Id} not found.", userId);
                await transaction.RollbackAsync();
                return new UserProfileResponse()
                {
                    Succeeded = false,
                    Message = "User not found."
                };
            }

            logger.LogInformation("Updating profile information for user with email: {Email}.", user.Email);

            if (string.IsNullOrWhiteSpace(request.FirstName) is false)
                user.FirstName = request.FirstName;

            if (string.IsNullOrWhiteSpace(request.LastName) is false)
                user.LastName = request.LastName;

            if (string.IsNullOrWhiteSpace(request.UserName) is false)
                user.UserName = request.UserName;

            if (string.IsNullOrWhiteSpace(request.PhoneNumber) is false)
                user.PhoneNumber = request.PhoneNumber;

            if (string.IsNullOrWhiteSpace(request.DateOfBirth) is false)
                user.DateOfBirth = DateTime.Parse(request.DateOfBirth, new CultureInfo("en-GB"));

            if (string.IsNullOrWhiteSpace(request.ProfilePictureUrl) is false)
                user.ProfilePictureUrl = request.ProfilePictureUrl;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded is false)
            {
                logger.LogWarning(
                    "Failed to update profile information for user with email: {Email}. Errors: {@Errors}",
                            user.Email,
                            result.Errors);

                await transaction.RollbackAsync();
                return new UserProfileResponse()
                {
                    Succeeded = false,
                    Message = "Failed to update user profile.",
                    IdentityErrors = result.Errors
                };
            }

            user.LastUpdatedAt = DateTime.UtcNow;

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
            logger.LogError(ex,
                "An error occurred while updating profile information for user with Id: {Id}",
                        userId);

            await transaction.RollbackAsync();
            throw;
        }
    }
    public Task<UserProfileResponse> UpdateEmailAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileResponse> DeleteUserAccount()
    {
        throw new NotImplementedException();
    }
}

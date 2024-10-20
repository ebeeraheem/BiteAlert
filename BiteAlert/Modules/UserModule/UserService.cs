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
    public async Task<BaseResponse> SelectRoleAsync(string userId, string roleName)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found.", userId);

            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User not found.",
                Data = new { userId }
            };
        }

        if (await roleManager.RoleExistsAsync(roleName) is false)
        {
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Selected role does not exist.",
                Data = new { roleName }
            };
        }

        var userRoles = await userManager.GetRolesAsync(user);

        if (userRoles.Any() && userRoles.Contains("Admin") is false)
        {
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Role selection is not allowed.",
                Data = new { userId }
            };
        }

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded is false)
        {
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Failed to assign role.",
                Data = new { userId, result.Errors }
            };
        }

        user.LastUpdatedAt = DateTime.UtcNow;
        return new BaseResponse()
        {
            Succeeded = true,
            Message = "Role assigned successfully.",
            Data = new { userId }
        };
    }
    public async Task<BaseResponse> UpdateProfileAsync(string userId, UserProfileRequest request)
    {
        var transaction = await context.Database
        .BeginTransactionAsync();

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogWarning("User with Id {Id} not found.", userId);
            await transaction.RollbackAsync();
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "User not found.",
                Data = new { userId }
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
            return new BaseResponse()
            {
                Succeeded = false,
                Message = "Failed to update user profile.",
                Data = new { result.Errors }
            };
        }

        user.LastUpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new BaseResponse()
        {
            Succeeded = true,
            Message = "User profile updated successfully.",
            Data = new { user.Id }
        };
    }
    public Task<BaseResponse> UpdateEmailAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse> DeleteUserAccount()
    {
        throw new NotImplementedException();
    }
}


using BiteAlert.Infrastructure.Data;
using BiteAlert.Modules.Shared;
using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.UserModule;

public class UserService(ApplicationDbContext context,
                         UserManager<ApplicationUser> userManager,
                         ILogger<UserService> logger) : IUserService

{
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

                return new UserProfileResponse()
                {
                    Succeeded = false,
                    Message = "User not found."
                };
            }

            logger.LogInformation("Updating profile information for user with email: {Email}.", user.Email);

            if (request.FirstName is not null) user.FirstName = request.FirstName;
            if (request.LastName is not null) user.LastName = request.LastName;
            if (request.UserName is not null) user.UserName = request.UserName;
            if (request.PhoneNumber is not null) user.PhoneNumber = request.PhoneNumber;
            if (request.DateOfBirth is not null) user.DateOfBirth = request.DateOfBirth;
            if (request.ProfilePictureUrl is not null) user.ProfilePictureUrl = request.ProfilePictureUrl;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded is false)
            {
                logger.LogWarning(
                    "Failed to update profile information for user with email: {Email}. Errors: {@Errors}",
                            user.Email,
                            result.Errors);

                return new UserProfileResponse()
                {
                    Succeeded = false,
                    Message = "Failed to update user profile.",
                    IdentityErrors = result.Errors
                };
            }

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

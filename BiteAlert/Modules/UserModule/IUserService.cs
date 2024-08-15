namespace BiteAlert.Modules.UserModule;

public interface IUserService
{
    Task<UserProfileResponse> UpdateProfileAsync(string userId, UserProfileRequest request);
    Task<UserProfileResponse> UpdateEmailAsync();
    Task<UserProfileResponse> DeleteUserAccount();
}

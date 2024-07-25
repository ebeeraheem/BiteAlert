namespace BiteAlert.Modules.Authentication;
public interface IUserService
{
    Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request);
    Task<LoginUserResponse> LoginUserAsync(LoginUserRequest request);
    Task<UserProfileResponse> UpdateProfileAsync(string userId, UserProfileRequest request);
    Task<UserProfileResponse> UpdateEmailAsync();
    Task<UserProfileResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request);
    Task<UserProfileResponse> ResetPasswordAsync();
    Task<UserProfileResponse> VerifyEmailAsync();
    Task<UserProfileResponse> DeleteUserAccount();
}
namespace BiteAlert.Modules.Authentication;
public interface IUserService
{
    Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request);
    Task<LoginUserResponse> LoginUserAsync(LoginUserRequest request);
    Task<UserProfileResponse> UpdateProfileAsync();
    Task<UserProfileResponse> UpdateEmailAsync();
    Task<UserProfileResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request);
    Task<UserProfileResponse> ResetPasswordAsync();
}
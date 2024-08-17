// Ignore Spelling: Auth

namespace BiteAlert.Modules.AuthModule;
public interface IAuthService
{
    Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request);
    Task<LoginUserResponse> LoginUserAsync(LoginUserRequest request);
    Task<AuthResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request);
    Task<AuthResponse> ResetPasswordAsync();
    Task<AuthResponse> VerifyEmailAsync(string userId, string token);
}
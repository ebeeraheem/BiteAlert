// Ignore Spelling: Auth

namespace BiteAlert.Modules.AuthModule;
public interface IAuthService
{
    Task<AuthResponse> RegisterUserAsync(RegisterUserRequest request);
    Task<AuthResponse> LoginUserAsync(LoginUserRequest request);
    Task<AuthResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request);
    Task<AuthResponse> ResetPasswordAsync();
    Task<AuthResponse> SendVerificationEmailAsync(string userId);
    Task<AuthResponse> VerifyEmailAsync(string userId, string token);
}
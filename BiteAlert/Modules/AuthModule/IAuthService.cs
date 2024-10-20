// Ignore Spelling: Auth

using BiteAlert.Modules.Shared;

namespace BiteAlert.Modules.AuthModule;
public interface IAuthService
{
    Task<Shared.BaseResponse> RegisterUserAsync(RegisterUserRequest request);
    Task<Shared.BaseResponse> LoginUserAsync(LoginUserRequest request);
    Task<Shared.BaseResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request);
    Task<Shared.BaseResponse> SendPasswordResetEmail(string userId);
    Task<Shared.BaseResponse> ResetPasswordAsync(PasswordResetRequest request);
    Task<Shared.BaseResponse> SendVerificationEmailAsync(string userId);
    Task<Shared.BaseResponse> VerifyEmailAsync(string userId, string token);
}
// Ignore Spelling: Auth

using BiteAlert.Modules.Shared;

namespace BiteAlert.Modules.AuthModule;
public interface IAuthService
{
    Task<BaseResponse> RegisterUserAsync(RegisterUserRequest request);
    Task<BaseResponse> LoginUserAsync(LoginUserRequest request);
    Task<BaseResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request);
    Task<BaseResponse> SendPasswordResetEmail(string userId);
    Task<BaseResponse> ResetPasswordAsync(PasswordResetRequest request);
    Task<BaseResponse> SendVerificationEmailAsync(string userId);
    Task<BaseResponse> VerifyEmailAsync(string userId, string token);
}
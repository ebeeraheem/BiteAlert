using BiteAlert.Modules.Shared;

namespace BiteAlert.Modules.UserModule;

public interface IUserService
{
    Task<BaseResponse> SelectRoleAsync(string userId, string roleName);
    Task<BaseResponse> UpdateProfileAsync(string userId, UserProfileRequest request);
    Task<BaseResponse> UpdateEmailAsync();
    Task<BaseResponse> DeleteUserAccount();
}

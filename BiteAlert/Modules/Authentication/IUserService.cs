using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.Authentication;
public interface IUserService
{
    Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request);
}
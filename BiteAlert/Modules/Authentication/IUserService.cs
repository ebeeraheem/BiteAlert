namespace BiteAlert.Modules.Authentication;
public interface IUserService
{
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest request);
    Task<LoginResponse> LoginUserAsync(LoginRequest request);
}
using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.Authentication;

public class RegisterUserResponse
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public IEnumerable<IdentityError>? Error { get; set; }
}

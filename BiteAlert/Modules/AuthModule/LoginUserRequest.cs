using System.ComponentModel.DataAnnotations;

namespace BiteAlert.Modules.AuthModule;

public class LoginUserRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}

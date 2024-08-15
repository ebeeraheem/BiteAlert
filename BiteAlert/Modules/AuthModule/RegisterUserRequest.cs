using System.ComponentModel.DataAnnotations;

namespace BiteAlert.Modules.AuthModule;

public class RegisterUserRequest
{
    public required string UserName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}

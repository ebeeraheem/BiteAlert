using System.ComponentModel.DataAnnotations;

namespace BiteAlert.Modules.Authentication;

public class LoginRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}

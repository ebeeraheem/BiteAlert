using System.ComponentModel.DataAnnotations;

namespace BiteAlert.Modules.AuthModule;

public class VerifyEmailRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Token { get; set; }
}

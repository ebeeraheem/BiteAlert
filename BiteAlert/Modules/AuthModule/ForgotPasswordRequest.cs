using System.ComponentModel.DataAnnotations;

namespace BiteAlert.Modules.AuthModule;

public class ForgotPasswordRequest
{
    [EmailAddress]
    public required string Email { get; set; }
}

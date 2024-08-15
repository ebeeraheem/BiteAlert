// Ignore Spelling: Auth

namespace BiteAlert.Modules.AuthModule;

public class UpdatePasswordRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
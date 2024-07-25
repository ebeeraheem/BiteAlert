// Ignore Spelling: Auth

namespace BiteAlert.Modules.Authentication;

public class UpdatePasswordRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
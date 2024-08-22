namespace BiteAlert.Modules.AuthModule;

public class PasswordResetRequest
{
    public required string UserId { get; set; }
    public required string PasswordResetToken { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}

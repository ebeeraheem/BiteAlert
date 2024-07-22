namespace BiteAlert.Modules.Authentication;

public class LoginResponse
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
}

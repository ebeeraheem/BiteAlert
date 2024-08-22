using System.Text.Json.Serialization;

namespace BiteAlert.Modules.EmailModule.PasswordReset;

public class PasswordResetData
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("support_email")]
    public string SupportEmail { get; set; } = string.Empty;

    [JsonPropertyName("password_reset_link")]
    public string PasswordResetLink { get; set; } = string.Empty;
}

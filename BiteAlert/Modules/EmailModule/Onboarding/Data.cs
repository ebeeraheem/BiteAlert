using System.Text.Json.Serialization;

namespace BiteAlert.Modules.EmailModule.Onboarding;

public class Data
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("email_confirmation_token")]
    public string EmailConfirmationToken { get; set; } = string.Empty;

    [JsonPropertyName("support_email")]
    public string SupportEmail { get; set; } = string.Empty;
}

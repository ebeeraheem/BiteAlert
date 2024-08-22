// Ignore Spelling: Onboarding

using System.Text.Json.Serialization;

namespace BiteAlert.Modules.EmailModule.Onboarding;

public class OnboardingEmailData
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("email_confirmation_token")]
    public string EmailConfirmationLink { get; set; } = string.Empty;

    [JsonPropertyName("support_email")]
    public string SupportEmail { get; set; } = string.Empty;
}

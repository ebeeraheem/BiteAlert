using System.Text.Json.Serialization;

namespace BiteAlert.Modules.EmailModule.Onboarding;

public class Data
{
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("support_email")]
    public string SupportEmail { get; set; } = string.Empty;
}

// Ignore Spelling: Onboarding

using System.Text.Json.Serialization;

namespace BiteAlert.Modules.EmailModule;

public class EmailTemplate
{
    public From From { get; set; } = null!;
    public List<To> To { get; set; } = null!;
    public List<Personalization> Personalization { get; set; } = null!;

    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;
}

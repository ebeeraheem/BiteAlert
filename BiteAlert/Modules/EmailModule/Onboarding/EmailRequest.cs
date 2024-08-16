﻿// Ignore Spelling: Personalisation

using System.Text.Json.Serialization;

namespace BiteAlert.Modules.EmailModule.Onboarding;

public class EmailRequest : IEmailRequestBase
{
    public From From { get; set; } = null!;
    public List<To> To { get; set; } = null!;
    public Personalisation Personalisation { get; set; } = null!;

    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;
}

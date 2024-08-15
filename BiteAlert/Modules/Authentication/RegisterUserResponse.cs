using BiteAlert.Modules.Shared;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace BiteAlert.Modules.Authentication;

public class RegisterUserResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<IdentityError>? IdentityErrors { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<FluentValidationError>? FluentValidationErrors { get; set; }
}

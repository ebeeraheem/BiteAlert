// Ignore Spelling: Auth

using BiteAlert.Modules.Shared;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace BiteAlert.Modules.AuthModule;

public class AuthResponse : BaseResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<IdentityError>? IdentityErrors { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<FluentValidationError>? FluentValidationErrors { get; set; }
}

using BiteAlert.Modules.Shared;
using System.Text.Json.Serialization;

namespace BiteAlert.Modules.AuthModule;

public class LoginUserResponse : BaseResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<FluentValidationError>? FluentValidationErrors { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }
}

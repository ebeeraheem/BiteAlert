using System.Text.Json.Serialization;

namespace BiteAlert.Modules.Authentication;

public class LoginUserResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }
}

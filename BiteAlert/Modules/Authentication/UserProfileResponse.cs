using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace BiteAlert.Modules.Authentication;

public class UserProfileResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<IdentityError>? Errors { get; set; }
}

using BiteAlert.Modules.Shared;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace BiteAlert.Modules.Authentication;

public class UserProfileResponse : BaseResponse
{

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<IdentityError>? Errors { get; set; }
}

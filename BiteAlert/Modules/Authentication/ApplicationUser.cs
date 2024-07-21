using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.Authentication;

public abstract class ApplicationUser : IdentityUser
{
    // Id, UserName, Email, Password and PhoneNumber
    // are inherited from IdentityUser
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}

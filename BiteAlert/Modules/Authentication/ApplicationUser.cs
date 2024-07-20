using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.Authentication;

public class ApplicationUser : IdentityUser
{
    // Id, Email, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed
    // and Password are inherited from IdentityUser
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public new string UserName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}

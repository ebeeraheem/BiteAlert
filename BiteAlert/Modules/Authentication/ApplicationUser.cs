using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.Authentication;

public class ApplicationUser : IdentityUser
{
    // Id, UserName, Email, PhoneNumber, PasswordHash, EmailConfirmed,
    // and PhoneNumberConfirmed are all inherited from IdentityUser
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}

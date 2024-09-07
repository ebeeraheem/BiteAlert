using Microsoft.AspNetCore.Identity;

namespace BiteAlert.Modules.Shared;

public class ApplicationUser : IdentityUser<Guid>
{
    // Id, PhoneNumber, PasswordHash, EmailConfirmed,
    // and PhoneNumberConfirmed are all inherited from IdentityUser
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public new required string UserName { get; set; }
    public new required string Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}

namespace BiteAlert.Modules.UserModule;

public class UserProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
}
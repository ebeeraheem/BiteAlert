// Ignore Spelling: App Tagline

using System.ComponentModel.DataAnnotations;

namespace BiteAlert.Modules.VendorModule;

public class RegisterVendorRequest
{
    public required string BusinessName { get; set; }
    public string? BusinessTagline { get; set; }
    public required string BusinessDescription { get; set; }
    public required string BusinessAddress { get; set; }

    [EmailAddress]
    public string? BusinessEmail { get; set; }

    [Phone]
    public string? BusinessPhoneNumber { get; set;}
    public string? LogoUrl { get; set; }
}

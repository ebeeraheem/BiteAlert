// Ignore Spelling: App Tagline Upsert

using System.ComponentModel.DataAnnotations;

namespace BiteAlert.Modules.VendorModule;

public class UpsertVendorRequest
{
    public required string BusinessName { get; set; }
    public string? BusinessTagline { get; set; }
    public required string BusinessDescription { get; set; }
    public required string BusinessAddress { get; set; }
    public string? BusinessEmail { get; set; }
    public string? BusinessPhoneNumber { get; set;}
}

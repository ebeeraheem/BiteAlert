namespace BiteAlert.Modules.VendorModule;

public class VendorRegistrationRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessDescription { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
}

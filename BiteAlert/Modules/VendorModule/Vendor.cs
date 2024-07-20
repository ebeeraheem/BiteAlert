using BiteAlert.Modules.Authentication;
using BiteAlert.Modules.ProductModule;
using BiteAlert.Modules.ReviewModule;

namespace BiteAlert.Modules.VendorModule;

public class Vendor : ApplicationUser
{
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessDescription { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; }

    // The vendor rating is the overall rating of a vendor computed from product ratings
    public double OverallRating { get; set; }
}

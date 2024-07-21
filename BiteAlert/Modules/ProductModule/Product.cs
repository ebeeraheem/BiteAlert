using BiteAlert.Modules.ReviewModule;
using BiteAlert.Modules.VendorModule;

namespace BiteAlert.Modules.ProductModule;

public class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string VendorId { get; set; } = string.Empty;
    public Vendor? Vendor { get; set; }
    public ICollection<Review>? Reviews { get; set; }
}

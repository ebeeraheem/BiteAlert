using BiteAlert.Modules.LikeModule;
using BiteAlert.Modules.ReviewModule;
using BiteAlert.Modules.VendorModule;

namespace BiteAlert.Modules.ProductModule;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string VendorId { get; set; } = string.Empty;
    public Vendor Vendor { get; set; } = new();
    public ICollection<Like> Likes { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}

using BiteAlert.Modules.VendorModule;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteAlert.Modules.ProductModule;

public class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    [Range(1.00, 10000000.00, ErrorMessage = "Price must be between ₦1 and ₦10,000,000.00")]
    public required decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }

    public required Guid VendorId { get; set; }
    public Vendor? Vendor { get; set; }
}

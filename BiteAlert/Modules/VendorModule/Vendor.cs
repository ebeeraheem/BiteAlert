using BiteAlert.Modules.Authentication;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteAlert.Modules.VendorModule;

public class Vendor
{
    [ForeignKey(nameof(User))]
    public Guid Id { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessDescription { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    
    //public ICollection<Product>? Products { get; set; }
    //public ICollection<Customer>? Followers { get; set; }

    //// The vendor rating is the overall rating of a vendor computed from product ratings
    //public double GetOverallRating()
    //{
    //    var products = Products;
    //    if (products is null) return 0;

    //    var reviews = products.SelectMany(
    //        r => r.Reviews ?? Enumerable.Empty<Review>());

    //    return reviews.Any() ? reviews.Average(r => (int)r.Rating) : 0;
    //}
}

namespace BiteAlert.Modules.VendorModule;

public class Vendor
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessDescription { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
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

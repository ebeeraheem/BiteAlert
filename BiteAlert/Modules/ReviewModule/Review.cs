using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.ProductModule;

namespace BiteAlert.Modules.ReviewModule;

public class Review
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public Customer? Customer { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public string? Comment { get; set; }
    public Rating Rating { get; set; }
    public DateTime ReviewedAt { get; set; }
}

public enum Rating
{
    OneStar = 1,
    TwoStars = 2,
    ThreeStars = 3,
    FourStars = 4,
    FiveStars = 5
}
using BiteAlert.Modules.CustomerModule;

namespace BiteAlert.Modules.ReviewModule;

public class Review
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public Customer Customer { get; set; } = new();
    public string Comment { get; set; } = string.Empty;
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
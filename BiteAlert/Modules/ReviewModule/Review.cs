using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.ProductModule;

namespace BiteAlert.Modules.ReviewModule;

public class Review
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public Customer Customer { get; set; } = new();
    public string ProductId { get; set; } = string.Empty;
    public Product Product { get; set; } = new();
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime ReviewedAt { get; set; }
}

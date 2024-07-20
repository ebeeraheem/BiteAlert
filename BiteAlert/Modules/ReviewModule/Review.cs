using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.ProductModule;

namespace BiteAlert.Modules.ReviewModule;

public class Review
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string ProductId { get; set; }
    public Product Product { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    public DateTime ReviewedAt { get; set; }
}

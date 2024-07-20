using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.ProductModule;

namespace BiteAlert.Modules.LikeModule;

public class Like
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public Customer Customer { get; set; } = new();
    public string ProductId { get; set; } = string.Empty;
    public Product Product { get; set; } = new();
    public DateTime LikedAt { get; set; }
}

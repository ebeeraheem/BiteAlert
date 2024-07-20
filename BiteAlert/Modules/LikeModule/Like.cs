using BiteAlert.Modules.CustomerModule;
using BiteAlert.Modules.ProductModule;

namespace BiteAlert.Modules.LikeModule;

public class Like
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = new();
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = new();
    public DateTime LikedAt { get; set; }
}

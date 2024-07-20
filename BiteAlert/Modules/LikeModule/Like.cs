using BiteAlert.Modules.CustomerModule;

namespace BiteAlert.Modules.LikeModule;

public class Like
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string ProductId { get; set; }
    public Product Product { get; set; }
    public DateTime LikedAt { get; set; }
}

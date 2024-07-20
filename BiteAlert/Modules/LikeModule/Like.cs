using BiteAlert.Modules.CustomerModule;

namespace BiteAlert.Modules.LikeModule;

public class Like
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public Customer Customer { get; set; } = new();
    public DateTime LikedAt { get; set; }
}

using BiteAlert.Modules.CustomerModule;

namespace BiteAlert.Modules.NotificationModule;

public class Notification
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public Customer Customer { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

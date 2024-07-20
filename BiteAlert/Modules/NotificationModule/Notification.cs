using BiteAlert.Modules.CustomerModule;

namespace BiteAlert.Modules.NotificationModule;

public class Notification
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

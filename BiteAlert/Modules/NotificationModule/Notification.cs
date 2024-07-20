using BiteAlert.Modules.CustomerModule;

namespace BiteAlert.Modules.NotificationModule;

public class Notification
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Add a NotificationType enumeration to easily classify notifications by type
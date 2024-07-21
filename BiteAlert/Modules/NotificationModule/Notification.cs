using BiteAlert.Modules.Authentication;

namespace BiteAlert.Modules.NotificationModule;

public class Notification
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public required ApplicationUser User { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Add a NotificationType enumeration to easily classify notifications by type
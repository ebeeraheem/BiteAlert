using BiteAlert.Modules.Authentication;

namespace BiteAlert.Modules.NotificationModule;

public class Notification
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Add a NotificationType enumeration to easily classify notifications by type
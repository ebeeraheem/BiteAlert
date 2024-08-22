using MediatR;

namespace BiteAlert.Modules.NotificationModule;

public class PasswordResetEvent : INotification
{
    public required Guid UserId { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PasswordResetToken { get; set; }
}

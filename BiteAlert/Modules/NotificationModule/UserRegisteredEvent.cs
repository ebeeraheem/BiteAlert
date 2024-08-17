namespace BiteAlert.Modules.NotificationModule;

public class UserRegisteredEvent
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
}

namespace BiteAlert.Modules.NotificationModule;

public interface IEventSubscriber<in TEvent>
{
    void Handle(TEvent eventToHandle);
}

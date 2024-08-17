namespace BiteAlert.Modules.NotificationModule;

public interface IEventPublisher
{
    void Publish<TEvent>(TEvent eventToPublish);
    void Subscribe<TEvent>(IEventSubscriber<TEvent> subscriber);
}

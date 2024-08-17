namespace BiteAlert.Modules.NotificationModule;

public class EventPublisher : IEventPublisher
{
    private readonly List<object> _subscribers = [];

    public void Publish<TEvent>(TEvent eventToPublish)
    {
        foreach (var subscriber in _subscribers.OfType<IEventSubscriber<TEvent>>())
        {
            subscriber.Handle(eventToPublish);
        }
    }

    public void Subscribe<TEvent>(IEventSubscriber<TEvent> subscriber)
    {
        _subscribers.Add(subscriber);
    }
}

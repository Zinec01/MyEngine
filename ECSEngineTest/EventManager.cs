namespace ECSEngineTest;

public static class EventManager
{
    private static readonly SortedDictionary<EventTypeFlags, HashSet<IEventSubscriber>> _subscribers = [];

    public static void RaiseEvent(EventTypeFlags eventType, object? data)
    {

    }

    public static void Subscribe(this IEventSubscriber subscriber, EventTypeFlags eventType)
    {

    }

    public static void SubscribeOnce(this IEventSubscriber subscriber, EventTypeFlags eventType, Action eventAction)
    {

    }

    public static void Unsubscribe(this IEventSubscriber subscriber, EventTypeFlags eventType)
    {

    }
}

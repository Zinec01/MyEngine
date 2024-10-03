namespace ECSEngineTest;

public interface IEventSubscriber
{
    void OnEvent(EventTypeFlags eventType, object? data);
}

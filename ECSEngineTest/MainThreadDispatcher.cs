using System.Collections.Concurrent;

namespace ECSEngineTest;

public static class MainThreadDispatcher
{
    private static readonly ConcurrentQueue<Action> _mainThreadActions = new();

    public static void Enqueue(Action action)
    {
        _mainThreadActions.Enqueue(action);
    }

    public static void ExecuteOnMainThread()
    {
        while (_mainThreadActions.TryDequeue(out var action))
        {
            Console.WriteLine("Thread ID in queue execution: " + Environment.CurrentManagedThreadId);
            SynchronizationContext.Current?.Post(_ => action?.Invoke(), null);
            //action?.Invoke();
        }
    }
}

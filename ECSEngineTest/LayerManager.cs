namespace ECSEngineTest;

public static class LayerManager
{
    private static readonly List<Layer> _layers = [];
    public static IReadOnlyList<Layer> Layers => _layers;

    public static void AddLayer(Layer layer)
    {
        _layers.Add(layer);
    }

    public static void AddOverlay(Layer overlay)
    {
        _layers.Insert(0, overlay);
    }

    public static void RemoveLayer(Layer layer)
    {
        _layers.Remove(layer);
    }

    internal static void Update(double deltaTime, double time)
    {
        var args = new LayerEventArgs(deltaTime, time);
        for (int i = _layers.Count - 1; i >= 0; i--)
        {
            var layer = _layers.ElementAt(i);
            if (layer is null || !layer.Enabled)
                continue;

            layer.OnUpdate(args);

            if (args.Cancel) break;
        }
    }

    internal static void Render(double deltaTime, double time)
    {
        var args = new LayerEventArgs(deltaTime, time);
        for (int i = _layers.Count - 1; i >= 0; i--)
        {
            var layer = _layers.ElementAt(i);
            if (layer is null || !layer.Enabled)
                continue;

            layer.OnRender(args);

            if (args.Cancel) break;
        }
    }

    internal static void RaiseEvent(EventTypeFlags eventType, EventEventArgs args)
    {
        for (int i = 0; i < _layers.Count; i++)
        {
            var layer = _layers.ElementAt(i);
            if (layer is null || !layer.Enabled)
                continue;

            layer.OnEvent(eventType, args);

            if (args.Cancel) break;
        }
    }
}

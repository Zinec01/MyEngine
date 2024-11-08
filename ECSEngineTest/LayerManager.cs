namespace ECSEngineTest;

internal static class LayerManager
{
    private static readonly SortedSet<Layer> _layers = new(new LayerComparer());
    public static IReadOnlyList<Layer> Layers => [.. _layers];

    public static bool AddLayer(Layer layer)
    {
        return _layers.Add(layer);
    }

    public static bool RemoveLayer(Layer layer)
    {
        return _layers.Remove(layer);
    }

    public static void Update(double deltaTime)
    {
        var args = new LayerEventArgs(deltaTime);
        for (int i = _layers.Count - 1; i >= 0; i--)
        {
            var layer = _layers.ElementAt(i);
            if (layer is null || !layer.Enabled)
                continue;

            layer.OnUpdate(args);

            if (args.Cancel) break;
        }
    }

    public static void Render(double deltaTime)
    {
        var args = new LayerEventArgs(deltaTime);
        for (int i = 0; i < _layers.Count; i++)
        {
            var layer = _layers.ElementAt(i);
            if (layer is null || !layer.Enabled)
                continue;

            layer.OnRender(args);

            if (args.Cancel) break;
        }
    }
}

internal class LayerComparer : IComparer<Layer>
{
    public int Compare(Layer? x, Layer? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        return x.Order.CompareTo(y.Order);
    }
}
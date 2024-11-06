namespace ECSEngineTest;

public abstract class Layer(string name, int order, bool enabled = true)
{
    public string Name { get; } = name;
    public int Order { get; } = order;
    public bool Enabled { get; internal set; } = enabled;

    internal abstract void OnUpdate(object? sender, LayerEventArgs args);
    internal abstract void OnRender(object? sender, LayerEventArgs args);
}

internal class LayerEventArgs(double deltaTime) : EventArgs
{
    internal double DeltaTime { get; } = deltaTime;
    internal bool Cancel { get; set; } = false;
}

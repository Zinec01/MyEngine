namespace ECSEngineTest;

public abstract class Layer(string name)
{
    public string Name { get; } = name;
    public bool Enabled { get; set; } = true;

    internal virtual void OnInit() { }
    internal virtual void OnUpdate(LayerEventArgs args) { }
    internal virtual void OnRender(LayerEventArgs args) { }
    internal virtual void OnEvent(EventTypeFlags eventType, EventEventArgs args) { }
}

public class LayerEventArgs(double deltaTime, double time) : EventArgs
{
    public double DeltaTime { get; } = deltaTime;
    public double Time { get; } = time;
    public bool Cancel { get; set; } = false;
}

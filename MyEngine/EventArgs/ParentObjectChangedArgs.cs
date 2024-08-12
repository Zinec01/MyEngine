using MyEngine.Interfaces;

namespace MyEngine.EventArgs;

public class ParentObjectChangedArgs(ITransformable parent, float deltaTime, ObjectChangedFlag changeEvent) : System.EventArgs
{
    public ITransformable Parent { get; } = parent;
    public float DeltaTime { get; } = deltaTime;
    public ObjectChangedFlag ChangeEvent { get; } = changeEvent;
}

using System.Numerics;

namespace MyEngine.Interfaces;

public interface IMovable
{
    public Vector3 Position { get; }

    public event EventHandler<ObjectChangedFlag> Moved;

    public void SubscribeTo(IMovable @object);
}

public enum ObjectChangedFlag
{
    POSITION,
    ROTATION,
    SCALE,
    OPACITY
}

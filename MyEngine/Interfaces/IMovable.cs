using System.Numerics;

namespace MyEngine.Interfaces;

public interface IMovable
{
    public Vector3 Position { get; }
    public Quaternion Rotation { get; }

    public event EventHandler<ObjectChangedFlag> Moved;

    public void SubscribeTo(IMovable @object, Action<IMovable, ObjectChangedFlag> updateAction)
    {
        @object.Moved += (sender, flags) =>
        {
            if (sender is IMovable obj)
                updateAction?.Invoke(obj, flags);
        };
    }
}

public enum ObjectChangedFlag
{
    NONE = 1,
    POSITION = 10,
    ROTATION = 100,
    SCALE = 1_000,
    OPACITY = 10_000,
    COLOR = 100_000
}

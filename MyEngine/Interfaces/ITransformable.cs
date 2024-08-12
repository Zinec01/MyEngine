using System.Numerics;

namespace MyEngine.Interfaces;

public interface ITransformable
{
    public Vector3 PreviousPosition { get; }
    public Vector3 CurrentPosition { get; }
    public Vector3 TargetPosition { get; }
    public Quaternion PreviousRotation { get; }
    public Quaternion CurrentRotation { get; }
    public Quaternion TargetRotation { get; }
    public float PreviousScale { get; }
    public float CurrentScale { get; }
    public float TargetScale { get; }

    public event EventHandler<float> PositionChanged;
    public event EventHandler<float> RotationChanged;
    public event EventHandler<float> ScaleChanged;
}

public enum ObjectChangedFlag
{
    NONE = 0,
    POSITION = 1,
    ROTATION = 10,
    SCALE = 100,
    OPACITY = 1_000,
    COLOR = 10_000
}

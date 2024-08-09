using MyEngine.Interfaces;
using System.Numerics;

namespace MyEngine;

internal abstract class TransformObject : ITransformable
{
    public Vector3 PreviousPosition { get; protected set; } = Vector3.Zero;
    public Vector3 TargetPosition { get; protected set; } = Vector3.Zero;
    public Vector3 CurrentPosition { get; protected set; } = Vector3.Zero;
    public Quaternion PreviousRotation { get; protected set; } = Quaternion.Identity;
    public Quaternion TargetRotation { get; protected set; } = Quaternion.Identity;
    public Quaternion CurrentRotation { get; protected set; } = Quaternion.Identity;
    public float PreviousScale { get; protected set; } = 1f;
    public float TargetScale { get; protected set; } = 1f;
    public float CurrentScale { get; protected set; } = 1f;

    public virtual event EventHandler OnPositionChanged;
    public virtual event EventHandler OnRotationChanged;
    public virtual event EventHandler OnScaleChanged;

    public abstract void Update(float deltaTime);

    public virtual void Move(Vector3 position)
    {
        TargetPosition += position;
    }

    public virtual void SetPosition(Vector3 position)
    {
        CurrentPosition = TargetPosition = position;
    }

    public virtual void ChangeScale(float scale)
    {
        TargetScale = scale;
    }

    public virtual void SetScale(float scale)
    {
        CurrentScale = TargetScale = scale;
    }

    public virtual void Rotate(Quaternion rotation)
    {
        TargetRotation *= rotation;
    }

    public virtual void SetRotation(Quaternion rotation)
    {
        CurrentRotation = TargetRotation = rotation;
    }
}

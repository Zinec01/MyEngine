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

    public virtual event EventHandler<float> PositionChanged;
    public virtual event EventHandler<float> RotationChanged;
    public virtual event EventHandler<float> ScaleChanged;

    public virtual void Update(float deltaTime)
    {
        UpdatePosition(deltaTime);
        UpdateRotation(deltaTime);
        UpdateScale(deltaTime);
    }

    protected virtual void UpdatePosition(float deltaTime)
    {
        if (CurrentPosition != TargetPosition)
        {
            OnCurrentToTargetPositionTransition(deltaTime);
            PositionChanged?.Invoke(this, deltaTime);
        }
    }

    protected virtual void UpdateRotation(float deltaTime)
    {
        if (CurrentRotation != TargetRotation)
        {
            OnCurrentToTargetRotationTransition(deltaTime);
            RotationChanged?.Invoke(this, deltaTime);
        }
    }

    protected virtual void UpdateScale(float deltaTime)
    {
        if (CurrentScale != TargetScale)
        {
            OnCurrentToTargetScaleTransition(deltaTime);
            ScaleChanged?.Invoke(this, deltaTime);
        }
    }

    protected virtual void OnCurrentToTargetPositionTransition(float deltaTime)
    {
        PreviousPosition = CurrentPosition;
        CurrentPosition = Vector3.Lerp(CurrentPosition, TargetPosition, deltaTime);
    }

    protected virtual void OnCurrentToTargetRotationTransition(float deltaTime)
    {
        PreviousRotation = CurrentRotation;
        CurrentRotation = Quaternion.Slerp(CurrentRotation, TargetRotation, deltaTime);
    }

    protected virtual void OnCurrentToTargetScaleTransition(float deltaTime)
    {
        PreviousScale = CurrentScale;
        CurrentScale = CurrentScale.Lerp(TargetScale, deltaTime);
    }

    public virtual void MoveBy(Vector3 position)
    {
        TargetPosition += position;
    }

    public virtual void MoveTo(Vector3 position)
    {
        TargetPosition = position;
    }

    public virtual void SetPosition(Vector3 position)
    {
        CurrentPosition = TargetPosition = position;
    }

    public virtual void Rotate(Quaternion rotation)
    {
        TargetRotation *= rotation;
    }

    public virtual void SetRotation(Quaternion rotation)
    {
        CurrentRotation =  TargetRotation = rotation;
    }

    public virtual void ChangeScale(float scale)
    {
        TargetScale = scale;
    }

    public virtual void SetScale(float scale)
    {
        CurrentScale = TargetScale = scale;
    }
}

using MyEngine.Interfaces;
using System.Numerics;

namespace MyEngine;

public abstract class TransformObject : ITransformable
{
    public Vector3 PreviousPosition { get; protected set; }
    public Vector3 TargetPosition { get; protected set; }
    public Vector3 CurrentPosition { get; protected set; }
    public Quaternion PreviousRotation { get; protected set; }
    public Quaternion TargetRotation { get; protected set; }
    public Quaternion CurrentRotation { get; protected set; }
    public float PreviousScale { get; protected set; }
    public float TargetScale { get; protected set; }
    public float CurrentScale { get; protected set; }

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

        if (Vector3.Distance(CurrentPosition, TargetPosition) < 0.001f)
        {
            CurrentRotation = TargetRotation;
        }
    }

    protected virtual void OnCurrentToTargetRotationTransition(float deltaTime)
    {
        PreviousRotation = CurrentRotation;
        CurrentRotation = Quaternion.Slerp(CurrentRotation, TargetRotation, deltaTime);

        if (float.Abs(Quaternion.Dot(TargetRotation, CurrentRotation)) > 0.9985f)
        {
            CurrentRotation = TargetRotation;
        }
    }

    protected virtual void OnCurrentToTargetScaleTransition(float deltaTime)
    {
        PreviousScale = CurrentScale;
        CurrentScale = CurrentScale.Lerp(TargetScale, deltaTime);

        if (float.Abs(TargetScale - CurrentScale) < 0.001f)
        {
            CurrentScale = TargetScale;
        }
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
        PreviousPosition = CurrentPosition;
        CurrentPosition = TargetPosition = position;
    }

    public virtual void Rotate(Quaternion rotation)
    {
        TargetRotation *= rotation;
    }

    public virtual void SetRotation(Quaternion rotation)
    {
        PreviousRotation = CurrentRotation;
        CurrentRotation = TargetRotation = rotation;
    }

    public virtual void ChangeScale(float scale)
    {
        TargetScale = scale;
    }

    public virtual void SetScale(float scale)
    {
        PreviousScale = CurrentScale;
        CurrentScale = TargetScale = scale;
    }
}

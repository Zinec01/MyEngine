using MyEngine.Interfaces;
using System.Numerics;

namespace MyEngine;

public abstract class TransformObject : ITransformable
{
    protected Vector3? _previousPosition = null;
    protected Quaternion? _previousRotation = null;
    protected float? _previousScale = null;

    public Vector3 PreviousPosition { get; protected set; }
    public Vector3 TargetPosition { get; protected set; }
    public Vector3 CurrentPosition { get; protected set; }
    public Quaternion PreviousRotation { get; protected set; }
    public Quaternion TargetRotation { get; protected set; }
    public Quaternion CurrentRotation { get; protected set; }
    public float PreviousScale { get; protected set; }
    public float TargetScale { get; protected set; }
    public float CurrentScale { get; protected set; }

    public event EventHandler<float> PositionChanged;
    public event EventHandler<float> RotationChanged;
    public event EventHandler<float> ScaleChanged;

    public virtual void Update(float deltaTime)
    {
        UpdatePosition(deltaTime);
        UpdateRotation(deltaTime);
        UpdateScale(deltaTime);
    }

    protected virtual void UpdatePosition(float deltaTime)
    {
        if (_previousPosition.HasValue || PreviousPosition != CurrentPosition)
        {
            PreviousPosition = _previousPosition ?? CurrentPosition;
            _previousPosition = null;
            PreviousPositionChanged(deltaTime);
        }

        if (CurrentPosition != TargetPosition)
        {
            LerpPosition(deltaTime);
            CurrentPositionChanged(deltaTime);
        }
    }

    protected virtual void UpdateRotation(float deltaTime)
    {
        if (_previousRotation.HasValue || PreviousRotation != CurrentRotation)
        {
            PreviousRotation = _previousRotation ?? CurrentRotation;
            _previousRotation = null;
            PreviousRotationChanged(deltaTime);
        }

        if (CurrentRotation != TargetRotation)
        {
            LerpRotation(deltaTime);
            CurrentRotationChanged(deltaTime);
        }
    }

    protected virtual void UpdateScale(float deltaTime)
    {
        if (_previousScale.HasValue || PreviousScale != CurrentScale)
        {
            PreviousScale = _previousScale ?? CurrentScale;
            _previousScale = null;
            PreviousScaleChanged(deltaTime);
        }

        if (CurrentScale != TargetScale)
        {
            LerpScale(deltaTime);
            CurrentScaleChanged(deltaTime);
        }
    }

    protected virtual void LerpPosition(float deltaTime)
    {
        CurrentPosition = Vector3.Lerp(CurrentPosition, TargetPosition, deltaTime);

        if (Vector3.Distance(CurrentPosition, TargetPosition) < 0.001f)
        {
            CurrentPosition = TargetPosition;
        }
    }

    protected virtual void LerpRotation(float deltaTime)
    {
        CurrentRotation = Quaternion.Slerp(CurrentRotation, TargetRotation, deltaTime);

        if (float.Abs(Quaternion.Dot(TargetRotation, CurrentRotation)) > 0.9985f)
        {
            CurrentRotation = TargetRotation;
        }
    }

    protected virtual void LerpScale(float deltaTime)
    {
        CurrentScale = CurrentScale.Lerp(TargetScale, deltaTime);

        if (float.Abs(TargetScale - CurrentScale) < 0.001f)
        {
            CurrentScale = TargetScale;
        }
    }

    protected virtual void PreviousPositionChanged(float deltaTime) { }
    protected virtual void PreviousRotationChanged(float deltaTime) { }
    protected virtual void PreviousScaleChanged(float deltaTime) { }

    protected virtual void CurrentPositionChanged(float deltaTime)
    {
        PositionChanged?.Invoke(this, deltaTime);
    }

    protected virtual void CurrentRotationChanged(float deltaTime)
    {
        RotationChanged?.Invoke(this, deltaTime);
    }

    protected virtual void CurrentScaleChanged(float deltaTime)
    {
        ScaleChanged?.Invoke(this, deltaTime);
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
        _previousPosition ??= CurrentPosition;
        CurrentPosition = TargetPosition = position;
    }

    public virtual void Rotate(Quaternion rotation)
    {
        TargetRotation *= rotation;
    }

    public virtual void SetRotation(Quaternion rotation)
    {
        _previousRotation ??= CurrentRotation;
        CurrentRotation = TargetRotation = rotation;
    }

    public virtual void Rotate(Quaternion rotation, Vector3 rotateAround)
    {
        SetPosition(rotateAround + Vector3.Transform(TargetPosition - rotateAround, rotation));
        Rotate(rotation);
    }

    public virtual void ChangeScale(float scale)
    {
        TargetScale = scale;
    }

    public virtual void SetScale(float scale)
    {
        _previousScale ??= CurrentScale;
        CurrentScale = TargetScale = scale;
    }
}

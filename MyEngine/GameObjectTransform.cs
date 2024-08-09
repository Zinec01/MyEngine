using System.Numerics;

namespace MyEngine;

internal class GameObjectTransform : TransformObject
{
    public override event EventHandler OnPositionChanged;
    public override event EventHandler OnRotationChanged;
    public override event EventHandler OnScaleChanged;

    public bool ModelTransformPending { get; private set; } = true;

    private Matrix4x4 modelMat = Matrix4x4.Identity;
    public Matrix4x4 ModelMat
    {
        get
        {
            if (ModelTransformPending)
            {
                modelMat = Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(CurrentRotation) * Matrix4x4.CreateScale(CurrentScale) * Matrix4x4.CreateTranslation(CurrentPosition);
                ModelTransformPending = false;
            }

            return modelMat;
        }
    }

    public override void Update(float deltaTime)
    {
        if (CurrentPosition != TargetPosition)
        {
            PreviousPosition = CurrentPosition;
            CurrentPosition = Vector3.Lerp(CurrentPosition, TargetPosition, deltaTime);

            OnPositionChanged?.Invoke(this, System.EventArgs.Empty);
            ModelTransformPending = true;
        }
        if (CurrentRotation != TargetRotation)
        {
            PreviousRotation = CurrentRotation;
            CurrentRotation = Quaternion.Slerp(CurrentRotation, TargetRotation, deltaTime);

            OnRotationChanged?.Invoke(this, System.EventArgs.Empty);
            ModelTransformPending = true;
        }
        if (CurrentScale != TargetScale)
        {
            PreviousScale = CurrentScale;
            CurrentScale = CurrentScale.Lerp(TargetScale, deltaTime);

            OnScaleChanged?.Invoke(this, System.EventArgs.Empty);
            ModelTransformPending = true;
        }
    }

    public virtual void Rotate(Quaternion rotation, Vector3 rotateAround)
    {
        TargetRotation *= rotation;
    }

    public virtual void SetRotation(Quaternion rotation, Vector3 rotateAround)
    {
        CurrentRotation = TargetRotation = rotation;
        ModelTransformPending = true;
    }

    public override void SetPosition(Vector3 position)
    {
        base.SetPosition(position);
        ModelTransformPending = true;
    }

    public override void SetRotation(Quaternion rotation)
    {
        base.SetRotation(rotation);
        ModelTransformPending = true;
    }

    public override void SetScale(float scale)
    {
        base.SetScale(scale);
        ModelTransformPending = true;
    }
}

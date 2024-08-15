using System.Numerics;

namespace MyEngine;

public class GameObjectTransform : TransformObject
{
    public bool ModelTransformPending { get; private set; } = true;

    public Matrix4x4 ModelMat { get; protected set; } = Matrix4x4.Identity;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (ModelTransformPending)
        {
            ModelMat = Matrix4x4.Identity * Matrix4x4.CreateScale(CurrentScale) * Matrix4x4.CreateFromQuaternion(CurrentRotation) * Matrix4x4.CreateTranslation(CurrentPosition);
            ModelTransformPending = false;
        }
    }

    protected override void OnCurrentToTargetPositionTransition(float deltaTime)
    {
        base.OnCurrentToTargetPositionTransition(deltaTime);
        ModelTransformPending = true;
    }

    protected override void OnCurrentToTargetRotationTransition(float deltaTime)
    {
        base.OnCurrentToTargetRotationTransition(deltaTime);
        ModelTransformPending = true;
    }

    protected override void OnCurrentToTargetScaleTransition(float deltaTime)
    {
        base.OnCurrentToTargetScaleTransition(deltaTime);
        ModelTransformPending = true;
    }

    public override void SetPosition(Vector3 position)
    {
        PreviousPosition = CurrentPosition;
        CurrentPosition = TargetPosition = position;
        ModelTransformPending = true;
    }

    public void Rotate(Quaternion rotation, Vector3 rotateAround)
    {
        TargetPosition = rotateAround + Vector3.Transform(TargetPosition - rotateAround, rotation);
    }

    public void SetRotation(Quaternion rotation, Vector3 rotateAround)
    {
        PreviousRotation = CurrentRotation;

        var relativePos = TargetPosition - rotateAround;
        var translatedPos = Vector3.Transform(relativePos, rotation);
        CurrentPosition = TargetPosition = rotateAround + translatedPos;

        ModelTransformPending = true;
    }

    public override void SetRotation(Quaternion rotation)
    {
        PreviousRotation = CurrentRotation;
        CurrentRotation = TargetRotation = rotation;
        ModelTransformPending = true;
    }

    public override void SetScale(float scale)
    {
        PreviousScale = CurrentScale;
        CurrentScale = TargetScale = scale;
        ModelTransformPending = true;
    }
}

using System.Numerics;

namespace MyEngine;

internal class GameObjectTransform : TransformObject
{
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
        CurrentPosition = TargetPosition = position;
        ModelTransformPending = true;
    }

    //TODO rotation around a point
    public void Rotate(Quaternion rotation, Vector3 rotateAround)
    {
        TargetRotation *= rotation;
    }

    public void SetRotation(Quaternion rotation, Vector3 rotateAround)
    {
        CurrentRotation = TargetRotation = rotation;
        ModelTransformPending = true;
    }

    public override void SetRotation(Quaternion rotation)
    {
        CurrentRotation = TargetRotation = rotation;
        ModelTransformPending = true;
    }

    public override void SetScale(float scale)
    {
        CurrentScale = TargetScale = scale;
        ModelTransformPending = true;
    }
}

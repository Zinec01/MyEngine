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
        var relativePosition = CurrentPosition - rotateAround;
        relativePosition = Vector3.Transform(relativePosition, rotation);

        TargetPosition = rotateAround + relativePosition;

        ModelTransformPending = true;
    }

    public void SetRotation(Quaternion rotation, Vector3 rotateAround)
    {
        // Step 1: Translate the object so that the pivot point is at the origin
        var directionToRotateAround = CurrentPosition - rotateAround;

        // Step 2: Apply the rotation
        var rotatedDirection = Vector3.Transform(directionToRotateAround, rotation);

        // Step 3: Translate the object back to its original location
        CurrentPosition = TargetPosition = rotateAround + rotatedDirection;

        // Step 4: Set the new rotation directly
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

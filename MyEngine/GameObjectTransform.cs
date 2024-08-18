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
            ModelMat = Matrix4x4.Identity
                     * Matrix4x4.CreateScale(CurrentScale)
                     * Matrix4x4.CreateFromQuaternion(CurrentRotation)
                     * Matrix4x4.CreateTranslation(CurrentPosition);

            ModelTransformPending = false;
        }
    }

    protected override void PreviousPositionChanged(float deltaTime)
    {
        base.PreviousPositionChanged(deltaTime);
        ModelTransformPending = true;
    }

    protected override void PreviousRotationChanged(float deltaTime)
    {
        base.PreviousRotationChanged(deltaTime);
        ModelTransformPending = true;
    }

    protected override void PreviousScaleChanged(float deltaTime)
    {
        base.PreviousScaleChanged(deltaTime);
        ModelTransformPending = true;
    }

    protected override void CurrentPositionChanged(float deltaTime)
    {
        base.CurrentPositionChanged(deltaTime);
        ModelTransformPending = true;
    }

    protected override void CurrentRotationChanged(float deltaTime)
    {
        base.CurrentRotationChanged(deltaTime);
        ModelTransformPending = true;
    }

    protected override void CurrentScaleChanged(float deltaTime)
    {
        base.CurrentScaleChanged(deltaTime);
        ModelTransformPending = true;
    }

    //public void Rotate(Quaternion rotation, Vector3 rotateAround)
    //{
    //    TargetPosition = rotateAround + Vector3.Transform(TargetPosition - rotateAround, rotation);
    //}
}

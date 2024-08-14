using System.Numerics;
using System.Reflection;

namespace MyEngine;

internal class GameObjectTransform : TransformObject
{
    private Matrix4x4 modelMat = Matrix4x4.Identity;

    public bool ModelTransformPending { get; private set; } = true;

    public Matrix4x4 ModelMat { get => modelMat; protected set { Console.WriteLine("ModelMat SET"); modelMat = value; }  }

    public override void Update(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        base.Update(deltaTime);

        if (ModelTransformPending)
        {
            ModelMat = Matrix4x4.Identity * Matrix4x4.CreateScale(CurrentScale) * Matrix4x4.CreateFromQuaternion(CurrentRotation) * Matrix4x4.CreateTranslation(CurrentPosition);
            ModelTransformPending = false;
        }
    }

    protected override void OnCurrentToTargetPositionTransition(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        base.OnCurrentToTargetPositionTransition(deltaTime);
        ModelTransformPending = true;
    }

    protected override void OnCurrentToTargetRotationTransition(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        base.OnCurrentToTargetRotationTransition(deltaTime);
        ModelTransformPending = true;
    }

    protected override void OnCurrentToTargetScaleTransition(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        base.OnCurrentToTargetScaleTransition(deltaTime);
        ModelTransformPending = true;
    }

    public override void SetPosition(Vector3 position)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        CurrentPosition = TargetPosition = position;
        ModelTransformPending = true;
    }

    //TODO rotation around a point
    public void Rotate(Quaternion rotation, Vector3 rotateAround)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name} (around a point)");

        TargetPosition = rotateAround + Vector3.Transform(TargetPosition - rotateAround, rotation);
    }

    public void SetRotation(Quaternion rotation, Vector3 rotateAround)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name} (around a point)");

        CurrentPosition = TargetPosition = rotateAround + Vector3.Transform(TargetPosition - rotateAround, rotation);

        ModelTransformPending = true;
    }

    public override void SetRotation(Quaternion rotation)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        CurrentRotation = TargetRotation = rotation;
        ModelTransformPending = true;
    }

    public override void SetScale(float scale)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        CurrentScale = TargetScale = scale;
        ModelTransformPending = true;
    }
}

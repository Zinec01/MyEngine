using MyEngine.Interfaces;
using System.Numerics;
using System.Reflection;

namespace MyEngine;

internal abstract class TransformObject : ITransformable
{
    private Vector3 previousPosition = Vector3.Zero;
    private Vector3 targetPosition = Vector3.Zero;
    private Vector3 currentPosition = Vector3.Zero;
    private Quaternion previousRotation = Quaternion.Identity;
    private Quaternion targetRotation = Quaternion.Identity;
    private Quaternion currentRotation = Quaternion.Identity;
    private float previousScale = 1f;
    private float targetScale = 1f;
    private float currentScale = 1f;

    public Vector3 PreviousPosition { get => previousPosition; protected set { Console.WriteLine("PreviousPosition SET"); previousPosition = value; } }
    public Vector3 TargetPosition
    {
        get => targetPosition;
        protected set
        {
            Console.WriteLine("TargetPosition SET");
            targetPosition = value;
        }
    }
    public Vector3 CurrentPosition { get => currentPosition; protected set {
            Console.WriteLine("CurrentPosition SET");
            currentPosition = value;
        } }
    public Quaternion PreviousRotation { get => previousRotation; protected set {
            Console.WriteLine("PreviousRotation SET");
            previousRotation = value;
        } }
    public Quaternion TargetRotation { get => targetRotation; protected set {
            Console.WriteLine("TargetRotation SET");
            targetRotation = value;
        } }
    public Quaternion CurrentRotation { get => currentRotation; protected set {
            Console.WriteLine("CurrentRotation SET");
            currentRotation = value;
        } }
    public float PreviousScale { get => previousScale; protected set {
            Console.WriteLine("PreviousScale SET");
            previousScale = value;
        } }
    public float TargetScale { get => targetScale; protected set {
            Console.WriteLine("TargetScale SET");
            targetScale = value;
        } }
    public float CurrentScale { get => currentScale; protected set {
            Console.WriteLine("CurrentScale SET");
            currentScale = value;
        } }
    public virtual event EventHandler<float> PositionChanged;
    public virtual event EventHandler<float> RotationChanged;
    public virtual event EventHandler<float> ScaleChanged;

    public virtual void Update(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        UpdatePosition(deltaTime);
        UpdateRotation(deltaTime);
        UpdateScale(deltaTime);
    }

    protected virtual void UpdatePosition(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        if (CurrentPosition != TargetPosition)
        {
            OnCurrentToTargetPositionTransition(deltaTime);
            PositionChanged?.Invoke(this, deltaTime);
        }
    }

    protected virtual void UpdateRotation(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        if (CurrentRotation != TargetRotation)
        {
            OnCurrentToTargetRotationTransition(deltaTime);
            RotationChanged?.Invoke(this, deltaTime);
        }
    }

    protected virtual void UpdateScale(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        if (CurrentScale != TargetScale)
        {
            OnCurrentToTargetScaleTransition(deltaTime);
            ScaleChanged?.Invoke(this, deltaTime);
        }
    }

    protected virtual void OnCurrentToTargetPositionTransition(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        PreviousPosition = CurrentPosition;
        CurrentPosition = Vector3.Lerp(CurrentPosition, TargetPosition, deltaTime);

        if (Vector3.Distance(CurrentPosition, TargetPosition) < 0.001f)
        {
            CurrentRotation = TargetRotation;
        }
    }

    protected virtual void OnCurrentToTargetRotationTransition(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        PreviousRotation = CurrentRotation;
        CurrentRotation = Quaternion.Slerp(CurrentRotation, TargetRotation, deltaTime);

        if (float.Abs(Quaternion.Dot(TargetRotation, CurrentRotation)) > 0.9985f)
        {
            CurrentRotation = TargetRotation;
        }
    }

    protected virtual void OnCurrentToTargetScaleTransition(float deltaTime)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        PreviousScale = CurrentScale;
        CurrentScale = CurrentScale.Lerp(TargetScale, deltaTime);

        if (float.Abs(TargetScale - CurrentScale) < 0.001f)
        {
            CurrentScale = TargetScale;
        }
    }

    public virtual void MoveBy(Vector3 position)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        TargetPosition += position;
    }

    public virtual void MoveTo(Vector3 position)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        TargetPosition = position;
    }

    public virtual void SetPosition(Vector3 position)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        CurrentPosition = TargetPosition = position;
    }

    public virtual void Rotate(Quaternion rotation)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        TargetRotation *= rotation;
    }

    public virtual void SetRotation(Quaternion rotation)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        CurrentRotation =  TargetRotation = rotation;
    }

    public virtual void ChangeScale(float scale)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        TargetScale = scale;
    }

    public virtual void SetScale(float scale)
    {
        var method = MethodBase.GetCurrentMethod()!;
        Console.WriteLine($"{method.DeclaringType!.Name}.{method.Name}");
        CurrentScale = TargetScale = scale;
    }
}

using System.Numerics;

namespace MyEngine;

internal class Transform
{
    private Vector3 TargetPosition { get; set; }
    private float TargetScale { get; set; }
    private Quaternion TargetRotation { get; set; }

    public Vector3 CurrentPosition { get; private set; } = new(0, 0, 0);
    public float CurrentScale { get; private set; } = 1f;
    public Quaternion CurrentRotation { get; private set; } = Quaternion.Identity;

    private Matrix4x4 modelMat = Matrix4x4.Identity;
    public Matrix4x4 ModelMat
    {
        get
        {
            if (TransformPending)
            {
                modelMat = Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(CurrentRotation) * Matrix4x4.CreateScale(CurrentScale) * Matrix4x4.CreateTranslation(CurrentPosition);
                TransformPending = false;
            }

            return modelMat;
        }
    }

    public bool TransformPending { get; private set; } = true;

    public Transform()
    {
        TargetPosition = CurrentPosition;
        TargetScale = CurrentScale;
        TargetRotation = CurrentRotation;
    }

    public void Update(float deltaTime)
    {
        if (CurrentPosition != TargetPosition)
        {
            CurrentPosition = Vector3.Lerp(CurrentPosition, TargetPosition, deltaTime);
            TransformPending = true;
        }
        if (CurrentScale != TargetScale)
        {
            CurrentScale = CurrentScale.Lerp(TargetScale, deltaTime);
            TransformPending = true;
        }
        if (CurrentRotation != TargetRotation)
        {
            CurrentRotation = Quaternion.Slerp(CurrentRotation, TargetRotation, deltaTime);
            TransformPending = true;
        }
    }

    public void Move(Vector3 position)
    {
        TargetPosition += position;
        TransformPending = true;
    }

    public void SetPosition(Vector3 position)
    {
        CurrentPosition = TargetPosition = position;
        TransformPending = true;
    }

    public void ChangeScale(float scale)
    {
        TargetScale = scale;
        TransformPending = true;
    }

    public void SetScale(float scale)
    {
        CurrentScale = TargetScale = scale;
        TransformPending = true;
    }

    public void Rotate(Quaternion rotation)
    {
        TargetRotation *= rotation;
        TransformPending = true;
    }

    public void SetRotation(Quaternion rotation)
    {
        CurrentRotation = TargetRotation *= rotation;
        TransformPending = true;
    }
}

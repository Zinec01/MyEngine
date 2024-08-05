using System.Numerics;

namespace MyEngine;

internal class Transform
{
    public Vector3 Position { get; private set; } = new(0, 0, 0);
    public float Scale { get; private set; } = 1f;
    public Quaternion Rotation { get; private set; } = Quaternion.Identity;

    private Matrix4x4 modelMat = Matrix4x4.Identity;
    public Matrix4x4 ModelMat
    {
        get
        {
            if (TransformPending)
            {
                modelMat = Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);
                TransformPending = false;
            }

            return modelMat;
        }
    }

    public bool TransformPending { get; private set; } = true;

    public void Move(Vector3 position)
    {
        Position += position;
        TransformPending = true;
    }

    public void SetPosition(Vector3 position)
    {
        Position = position;
        TransformPending = true;
    }

    public void ChangeScale(float scale)
    {
        Scale = scale;
        TransformPending = true;
    }

    public void Rotate(Quaternion rotation)
    {
        Rotation *= rotation;
        TransformPending = true;
    }
}

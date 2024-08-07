using System.Numerics;

namespace MyEngine;

internal class Camera
{
    public Matrix4x4 ProjectMat { get; private set; } = Matrix4x4.Identity;
    public Matrix4x4 ViewMat { get; private set; } = Matrix4x4.Identity;
    public Vector3 Position { get; private set; }

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }

    public Vector3 LookDirection { get; private set; }

    public Camera(Vector3 position, Vector3 lookDir)
    {
        Position = position;
        LookDirection = lookDir;
    }

    public Camera() : this(new Vector3(0f, 0f, -3f), new Vector3(0f, 0f, 1f))
    {
    }
}

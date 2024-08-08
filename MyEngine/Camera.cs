using MyEngine.Interfaces;
using System.Numerics;

namespace MyEngine;

internal class Camera : IMovable
{
    public Matrix4x4 ProjectMat { get; private set; } = Matrix4x4.Identity;
    public Matrix4x4 ViewMat { get; private set; } = Matrix4x4.Identity;
    public Vector3 Position { get; private set; }
    public Vector3 Target { get; private set; }
    public Vector3 Direction { get; private set; }
    public Quaternion Rotation => throw new NotImplementedException();

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }

    public float FOV { get; private set; } = 90;

    private CameraTransform Transform { get; }

    public event EventHandler<ObjectChangedFlag> Moved;

    public Camera(Vector3 position, Vector3 target, float fov = 90)
    {
        Position = position;
        Target = target;
        FOV = fov;

        Transform = new CameraTransform();

        Direction = Vector3.Normalize(position - Target);

        ProjectMat = Matrix4x4.CreatePerspectiveFieldOfView(fov.DegToRad(), 1280f / 720f, 0.1f, 100f);

        var right = Vector3.Normalize(Vector3.Cross(new Vector3(0f, 1f, 0f), Direction));
        var up = Vector3.Cross(Direction, right);
        ViewMat = Matrix4x4.CreateLookAt(position, target, up);
    }

    public Camera() : this(new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 0f))
    {
    }

    public void Update(Vector3? position = null, Vector3? target = null, float? yaw = null, float? pitch = null)
    {

    }
}

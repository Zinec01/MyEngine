using Friflo.Engine.ECS;
using System.Numerics;

namespace ECSEngineTest.Components;

public struct CameraComponent() : IComponent
{
    public bool Active { get; set; } = false;

    public float AspectRatio { get; set; } = 16.0f / 9.0f;
    public float FieldOfView { get; set; } = 90.0f;
    public float NearPlane { get; set; } = 0.1f;
    public float FarPlane { get; set; } = 1000.0f;

    public Vector3 Front { get; set; } = new Vector3(0.0f, 0.0f, -1.0f);
    public Vector3 Up { get; set; } = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 Right { get; set; } = new Vector3(1.0f, 0.0f, 0.0f);

    public Matrix4x4 Projection { get; set; } = Matrix4x4.Identity;
}

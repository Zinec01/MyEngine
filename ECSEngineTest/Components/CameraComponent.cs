using Friflo.Engine.ECS;
using System.Numerics;

namespace ECSEngineTest.Components;

public struct CameraComponent : IComponent
{
    public bool Active { get; set; }

    public float AspectRatio { get; set; }
    public float FieldOfView { get; set; }
    public float NearPlane { get; set; }
    public float FarPlane { get; set; }

    public Matrix4x4 ProjectMat;
    public Matrix4x4 ViewMat;
}

using Friflo.Engine.ECS;
using System.Numerics;

namespace ECSEngineTest.Components;

public struct CameraComponent() : IComponent
{
    public bool Active { get; set; } = false;

    public Vector3 Front { get; set; } = Vector3.Zero;
    public Vector3 Up { get; set; } = Vector3.Zero;
    public Vector3 Right { get; set; } = Vector3.Zero;

    public Matrix4x4 Projection { get; set; } = Matrix4x4.Identity;
}

using Friflo.Engine.ECS;
using System.Numerics;

namespace ECSEngineTest.Components;

public struct TransformComponent : IComponent
{
    public Interpolatable<Vector3> Position { get; set; }
    public Interpolatable<Quaternion> Rotation { get; set; }
    public Interpolatable<float> Scale { get; set; }

    public Matrix4x4 WorldTransform { get; set; }
}

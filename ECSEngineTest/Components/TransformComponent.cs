using Friflo.Engine.ECS;
using System.Numerics;

namespace ECSEngineTest.Components;

public struct TransformComponent : IComponent
{
    public Interpolatable<Vector3> Position { get; set; }
    public Interpolatable<Quaternion> Rotation { get; set; }
    public Interpolatable<Vector3> Scale { get; set; }

    public Matrix4x4 WorldTransform { get; set; }

    public TransformComponent(Matrix4x4 transform)
    {
        WorldTransform = transform;

        Matrix4x4.Decompose(transform, out var scale, out var rotation, out var translation);

        Position = new Interpolatable<Vector3>(translation);
        Rotation = new Interpolatable<Quaternion>(rotation);
        Scale    = new Interpolatable<Vector3>(scale);
    }

    public TransformComponent(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Position = new Interpolatable<Vector3>(position);
        Rotation = new Interpolatable<Quaternion>(rotation);
        Scale    = new Interpolatable<Vector3>(scale);

        WorldTransform = Matrix4x4.Identity
                         * Matrix4x4.CreateScale(scale)
                         * Matrix4x4.CreateFromQuaternion(rotation)
                         * Matrix4x4.CreateTranslation(position);
    }
}

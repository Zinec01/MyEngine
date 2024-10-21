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

        Position = new Interpolatable<Vector3>(new Vector3(transform.M14, transform.M24, transform.M34));

        transform.M14 = transform.M24 = transform.M34 = 0.0f;

        var col1Len = new Vector3(transform.M11, transform.M21, transform.M31).Length();
        var col2Len = new Vector3(transform.M12, transform.M22, transform.M32).Length();
        var col3Len = new Vector3(transform.M13, transform.M23, transform.M33).Length();

        Scale = new Interpolatable<Vector3>(new Vector3(col1Len, col2Len, col3Len));

        transform.M11 /= col1Len;
        transform.M21 /= col1Len;
        transform.M31 /= col1Len;

        transform.M12 /= col2Len;
        transform.M22 /= col2Len;
        transform.M32 /= col2Len;

        transform.M13 /= col3Len;
        transform.M23 /= col3Len;
        transform.M33 /= col3Len;

        Rotation = new Interpolatable<Quaternion>(Quaternion.CreateFromRotationMatrix(transform));
    }
}

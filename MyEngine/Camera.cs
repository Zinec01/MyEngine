using Silk.NET.Maths;

namespace MyEngine;

internal class Camera : IDisposable
{
    public Matrix4X4<float> ProjectMat { get; private set; }
    public Matrix4X4<float> ViewMat { get; private set; }
    public Vector3D<float> Position { get; private set; }

    public void Dispose()
    {
    }
}

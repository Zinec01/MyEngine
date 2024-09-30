using System.Numerics;

namespace ECSEngineTest;

public struct VertexData
{
    public Vector3 Vertex;
    public Vector3 Normal;
}

public struct VertexTextureData
{
    public Vector3 Vertex;
    public Vector3 Normal;
    public Vector2 UVs;
}

using Friflo.Engine.ECS;

namespace ECSEngineTest.Components;

public readonly struct MeshComponent(uint vAO, uint vBO, uint eBO,
                                     VertexData[]? vertexData, VertexTextureData[]? vertexTextureData,
                                     uint[] indices) : IComponent
{
    public readonly uint VAO { get; } = vAO;
    public readonly uint VBO { get; } = vBO;
    public readonly uint EBO { get; } = eBO;

    public readonly VertexData[]? VertexData { get; } = vertexData;
    public readonly VertexTextureData[]? VertexTextureData { get; } = vertexTextureData;
    public readonly uint[] Indices { get; } = indices;
}

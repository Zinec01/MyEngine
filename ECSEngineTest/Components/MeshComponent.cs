using Friflo.Engine.ECS;

namespace ECSEngineTest.Components;

public readonly struct MeshComponent(uint vao, uint vbo, uint ebo,
                                     VertexData[]? vertexData, VertexTextureData[]? vertexTextureData,
                                     uint[] indices, int hash) : IComponent
{
    public readonly uint VAO { get; } = vao;
    public readonly uint VBO { get; } = vbo;
    public readonly uint EBO { get; } = ebo;

    public readonly VertexData[]? VertexData { get; } = vertexData;
    public readonly VertexTextureData[]? VertexTextureData { get; } = vertexTextureData;
    //public readonly float[] VertexData { get; } = vertexData;
    public readonly uint[] Indices { get; } = indices;

    internal readonly int Hash { get; } = hash;
}

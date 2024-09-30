using Friflo.Engine.ECS;

namespace ECSEngineTest.Components;

public struct ShaderProgramComponent(uint id, string name,
                                     uint vertexId, uint fragmentId,
                                     params uint[] otherIds) : IComponent
{
    public uint Id { get; } = id;
    public string Name { get; } = name;


    public uint VertexId { get; } = vertexId;
    public uint FragmentId { get; } = fragmentId;
    public uint[] OtherIds { get; } = otherIds;
}

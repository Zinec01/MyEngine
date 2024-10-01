using Friflo.Engine.ECS;

namespace ECSEngineTest.Components;

public struct ShaderProgramComponent(uint id, string name,
                                     uint vertexId, uint fragmentId,
                                     params uint[] otherIds) : IComponent
{
    public uint Id { get; set; } = id;
    public string Name { get; set; } = name;


    public uint VertexId { get; set; } = vertexId;
    public uint FragmentId { get; set; } = fragmentId;
    public uint[] OtherIds { get; set; } = otherIds;
}

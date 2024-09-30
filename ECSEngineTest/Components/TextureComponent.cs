using Friflo.Engine.ECS;

namespace ECSEngineTest.Components;

public struct TextureComponent(uint id, string filePath) : IComponent
{
    public uint Id { get; set; } = id;
    public string FilePath { get; set; } = filePath;
}

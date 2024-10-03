using Friflo.Engine.ECS;

namespace ECSEngineTest.Components;

public readonly struct TextureComponent(uint id, string filePath) : IComponent
{
    public uint Id { get; } = id;
    public string FilePath { get; } = filePath;
}

using Friflo.Engine.ECS;
using Silk.NET.OpenGL;

namespace ECSEngineTest;

public class Scene : IDisposable
{
    private static uint _idGen = 0;

    private readonly EntityStore _store;
    public SceneLoader Loader { get; }

    public uint Id { get; }
    public string Name { get; set; }
    public EntityFactory EntityFactory { get; }

    public Scene(string name)
    {
        Id = _idGen++;
        Name = name;

        _store = new();
        EntityFactory = new(_store);
        Loader = new(_store);
    }

    public void Dispose()
    {
        Loader.Dispose();
    }
}

using Friflo.Engine.ECS;

namespace ECSEngineTest;

public class Scene : IDisposable
{
    private static uint _idGen = 0;

    private readonly ParallelJobRunner _runner;
    private readonly EntityStore _store;

    public uint Id { get; }
    public string Name { get; set; }
    //public EntityFactory EntityFactory { get; }
    public SceneLoader Loader { get; }
    public ShaderManager ShaderManager { get; }

    public Scene(string name)
    {
        Id = _idGen++;
        Name = name;

        _runner = new ParallelJobRunner(Environment.ProcessorCount);
        _store = new EntityStore { JobRunner = _runner };

        //EntityFactory = new(_store);
        ShaderManager = new(_store);
        Loader = new(_store, ShaderManager);
    }

    internal void OnUpdate(object? sender, double deltaTime)
    {

    }

    internal void OnRender(object? sender, double deltaTime)
    {

    }

    public void Dispose()
    {
        Loader.Dispose();
        _runner.Dispose();
    }
}

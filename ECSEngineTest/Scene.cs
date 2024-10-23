using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using System.Diagnostics;

namespace ECSEngineTest;

public class Scene : IDisposable
{
    private static uint _idGen = 0;

    private readonly ParallelJobRunner _runner;
    private readonly EntityStore _store;
    private readonly SystemRoot _rootSystem;

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

        _rootSystem = new SystemRoot(_store)
        {
            //new TestSystem()
        };
    }

    internal void OnUpdate(object? sender, double deltaTime)
    {
        _rootSystem.Update(new UpdateTick((float)deltaTime, 0/*(float)(DateTime.Now - Application.AppStart).TotalSeconds*/));
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

public class TestSystem : QuerySystem<EntityName>
{
    protected override void OnUpdate()
    {
        Query.ForEach((Chunk<EntityName> components, ChunkEntities entities) =>
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Debug.WriteLine(components[i].value);
            }
        }).RunParallel();
    }
}

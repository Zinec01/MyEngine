using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Drawing;

namespace ECSEngineTest;

public class Scene : IDisposable
{
    private static uint _idGen = 0;

    private readonly ParallelJobRunner _runner;
    private readonly EntityStore _store;
    private readonly SystemRoot _rootSystem;

    public uint Id { get; }
    public string Name { get; set; }

    public EntityFactory EntityFactory { get; }
    public SceneLoader Loader { get; }
    public ShaderManager ShaderManager { get; }

    public Scene(string name)
    {
        Id = _idGen++;
        Name = name;

        _runner = new ParallelJobRunner(Environment.ProcessorCount);
        _store = new EntityStore { JobRunner = _runner };

        EntityFactory = new(_store);
        ShaderManager = new(_store);
        Loader = new(_store, ShaderManager, EntityFactory);

        _rootSystem = new SystemRoot(_store)
        {
            //new TestSystem()
        };
    }

    internal void OnUpdate(object? sender, double deltaTime)
    {
        _rootSystem.Update(new UpdateTick((float)deltaTime, (float)(DateTime.Now - Application.AppStart).TotalSeconds));
    }

    internal void OnRender(object? sender, double deltaTime)
    {
        Window.GL.ClearColor(Color.FromArgb(222, 235, 255));
        Window.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Renderer.RenderScene(_store, deltaTime);
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
            foreach (var entity in entities)
            {
                foreach (var script in entity.Scripts)
                {
                    script.Update();
                }
            }
        }).RunParallel();
        //Console.WriteLine($"Frame DeltaTime: {Tick.deltaTime * 1000:.000} ms");
    }
}

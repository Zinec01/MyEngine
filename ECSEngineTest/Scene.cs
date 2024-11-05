using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace ECSEngineTest;

public class Scene : Layer, IDisposable
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

    public Scene(string name) : base(name, 0)
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

        //EventManager.InputEvent += (sender, args) =>
        //{
        //    if (!args.EventType.HasFlag(EventTypeFlags.KeyboardEvent) || args.Data is not IKeyboard keyboard)
        //        return;

        //    if (keyboard.IsKeyPressed(Key.ControlLeft) && keyboard.IsKeyPressed(Key.R))
        //    {
        //        var query = _store.Query<EntityName>();
        //        if (query.Count > 0)
        //        {
        //            var cb = _store.GetCommandBuffer().Synced;
        //            query.ForEach((components, entities) =>
        //            {
        //                for (int i = 0; i < entities.Length; i++)
        //                {
        //                    if (components[i].value == "Main Camera")
        //                        continue;

        //                    cb.DeleteEntity(entities[i]);
        //                }
        //            }).RunParallel();

        //            Console.WriteLine("Thread ID inside event: " + Environment.CurrentManagedThreadId);
        //            MainThreadDispatcher.Enqueue(() =>
        //            {
        //                Console.WriteLine("Thread ID inside enqueued action: " + Environment.CurrentManagedThreadId);
        //                cb.Playback();
        //            });
        //        }
        //    }
        //};

        EventManager.MouseDown += (sender, args) =>
        {

        };
    }

    //internal void OnUpdate(double deltaTime)
    //{
    //    _rootSystem.Update(new UpdateTick((float)deltaTime, (float)(DateTime.Now - Application.AppStart).TotalSeconds));
    //}

    //internal void OnRender(double deltaTime)
    //{
    //    Window.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    //    Renderer.RenderScene(_store, deltaTime);
    //}

    internal override void OnUpdate(object? sender, LayerEventArgs args)
    {
        _rootSystem.Update(new UpdateTick((float)args.DeltaTime, (float)(DateTime.Now - Application.AppStart).TotalSeconds));
    }

    internal override void OnRender(object? sender, LayerEventArgs args)
    {
        Window.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Renderer.RenderScene(_store, args.DeltaTime);
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

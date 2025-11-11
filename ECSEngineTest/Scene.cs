using ECSEngineTest.Helpers;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Silk.NET.OpenGL;

namespace ECSEngineTest;

public class Scene : Layer, IDisposable
{
    private static uint _idGen = 0;

    private readonly ParallelJobRunner _runner;
    private readonly SystemRoot _rootSystem;

    public uint Id { get; }
    public string Name { get; set; }

    internal EntityStore EntityStore { get; }
    public EntityFactory EntityFactory { get; }
    public SceneLoader Loader { get; }
    public ShaderManager ShaderManager { get; }

    public Scene(string name) : base(name)
    {
        Id = _idGen++;
        Name = name;

        _runner = new ParallelJobRunner(Environment.ProcessorCount);
        EntityStore = new EntityStore { JobRunner = _runner };

        EntityFactory = new(EntityStore);
        ShaderManager = new(EntityStore);
        Loader = new(EntityStore, ShaderManager, EntityFactory);

        _rootSystem = new SystemRoot(EntityStore)
        {
            //new TestSystem()
        };

        EventManager.MouseDown += OnMouseDown;
        EventManager.MouseUp += OnMouseUp;
        EventManager.MouseClick += OnMouseClick;
        EventManager.MouseDoubleClick += OnMouseDoubleClick;
        EventManager.MouseScroll += OnMouseScroll;
        EventManager.KeyDown += OnKeyDown;
        EventManager.KeyUp += OnKeyUp;
        EventManager.WindowFileDrop += OnFileDrop;
    }

    internal override void OnUpdate(LayerEventArgs args)
    {
        _rootSystem.Update(new UpdateTick((float)args.DeltaTime, (float)args.Time));
    }

    internal override void OnRender(LayerEventArgs args)
    {
        Window.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Renderer.RenderScene(EntityStore, args.DeltaTime);
    }

    internal override void OnEvent(EventTypeFlags eventType, EventEventArgs args)
    {
        Console.WriteLine($"Scene:\tEvent - {eventType}");
    }

    private void OnMouseClick(object? sender, MouseClickEventArgs e)
    {
        Console.WriteLine($"Scene:\tMouse Click - {e.Button}");
    }

    private void OnMouseDoubleClick(object? sender, MouseClickEventArgs e)
    {
        Console.WriteLine($"Scene:\tMouse Double Click - {e.Button}");
    }

    private void OnMouseScroll(object? sender, MouseScrollEventArgs e)
    {
        Console.WriteLine($"Scene:\tMouse Scroll - X: {e.Scroll.X}, Y: {e.Scroll.Y}");
    }

    private void OnKeyDown(object? sender, KeyDownEventArgs e)
    {
        Console.WriteLine($"Scene:\tKey Down - {e.Keyboard.PressedKeys.Select(x => x.ToString()).Aggregate((c, n) => c + ", " + n)}");

        if (e.Keyboard.PressedKeys.Count == 2 && e.Keyboard.PressedKeys.Contains(Input.Key.ControlLeft) && e.Keyboard.PressedKeys.Contains(Input.Key.R))
        {
            var query = EntityStore.Query<EntityName>();
            if (query.Count > 0)
            {
                var cb = EntityStore.GetCommandBuffer().Synced;
                query.ForEach((components, entities) =>
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        if (components[i].value == "Main Camera")
                            continue;

                        cb.DeleteEntity(entities[i]);
                    }
                }).RunParallel();

                Console.WriteLine("Thread ID inside event: " + Environment.CurrentManagedThreadId);
                MainThreadDispatcher.Enqueue(() =>
                {
                    Console.WriteLine("Thread ID inside enqueued action: " + Environment.CurrentManagedThreadId);
                    cb.Playback();
                });
            }
        }
    }

    private void OnKeyUp(object? sender, KeyUpEventArgs e)
    {
        Console.WriteLine($"Scene:\tKey Down - {e.Key}");
    }

    private void OnFileDrop(object? sender, WindowFileDropEventArgs e)
    {
        Console.WriteLine($"Scene:\tFile Drop - {e.FilePaths.Select(x => Path.GetFileName(x)).Aggregate((c, n) => c + ", " + n)}");

        for (int i = 0; i < e.FilePaths.Length; i++)
        {
            var filePath = e.FilePaths[i];

            if (FileHelper.ValidateFilePath(ref filePath) && FileHelper.IsSupportedModelFile(filePath))
                Loader.LoadScene(filePath);
        }
    }

    public void OnMouseDown(object? sender, MouseDownEventArgs e)
    {
        Console.WriteLine($"Scene:\tMouse Down - {e.Mouse.PressedButtons.Select(x => x.ToString()).Aggregate((c, n) => c + ", " + n)}");
    }

    public void OnMouseUp(object? sender, MouseUpEventArgs e)
    {
        Console.WriteLine($"Scene:\tMouse Up - {e.Button}");
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

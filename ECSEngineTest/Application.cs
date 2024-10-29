using Silk.NET.OpenGL;

namespace ECSEngineTest;

public class Application : IDisposable
{
    private Window MainWindow { get; }

    public static DateTime AppStart { get; private set; }

    private readonly List<Scene> _scenes = [];
    public IReadOnlyList<Scene> Scenes => _scenes;
    public int? ActiveSceneId { get; set; }
    public Scene? ActiveScene => ActiveSceneId.HasValue ? _scenes.FirstOrDefault(x => x.Id == ActiveSceneId) : null;

    public event Action<Application> Init;

    public Application(WindowSettings windowSettings)
    {
        MainWindow = new(windowSettings);
        MainWindow.OnLoad += OnMainWindowLoad;
    }

    private void OnMainWindowLoad()
    {
        Window.GL.Enable(EnableCap.DepthTest);
        Window.GL.Enable(EnableCap.CullFace);
        Window.GL.CullFace(TriangleFace.Back);
        Window.GL.FrontFace(FrontFaceDirection.Ccw);

        ShaderUniforms.InitUniformBlocks();

        Init?.Invoke(this);
    }

    public Scene CreateScene(string name)
    {
        var scene = new Scene(name);

        MainWindow.OnRender += scene.OnRender;
        MainWindow.OnUpdate += scene.OnUpdate;
        
        _scenes.Add(scene);

        return scene;
    }

    public void Run()
    {
        AppStart = DateTime.Now;
        MainWindow?.Run();
    }

    public void Dispose()
    {
        foreach (var scene in _scenes)
        {
            scene.Dispose();
        }
        Window.GL.Dispose();
    }
}

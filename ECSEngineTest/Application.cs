using ECSEngineTest.Helpers;
using Silk.NET.OpenGL;

namespace ECSEngineTest;

public class Application : IDisposable
{
    private Window MainWindow { get; }

    public static DateTime AppStart { get; private set; }

    private readonly List<Scene> _scenes = [];
    public IReadOnlyList<Scene> Scenes => _scenes;
    public uint? ActiveSceneId { get; set; }
    public Scene? ActiveScene => ActiveSceneId.HasValue ? _scenes.FirstOrDefault(x => x.Id == ActiveSceneId) : null;

    public event Action<Application> Init;

    public Application(WindowSettings windowSettings)
    {
        MainWindow = new(windowSettings);
        MainWindow.OnLoad += OnMainWindowLoad;
        MainWindow.OnClosing += OnMainWindowClosing;
        MainWindow.OnFileDrop += OnMainWindowFileDrop;
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

    private void OnMainWindowClosing()
    {
        ShaderManager.Dispose();
        ShaderUniforms.Dispose();
        MeshManager.DisposeAllMeshes();
    }

    private void OnMainWindowFileDrop(string[] filePaths)
    {
        for (int i = 0; i < filePaths.Length; i++)
        {
            var filePath = filePaths[i];

            if (FileHelper.ValidateFilePath(ref filePath) && FileHelper.IsSupportedModelFile(filePath))
                ActiveScene?.Loader.LoadScene(filePath);
        }
    }

    public Scene CreateScene(string name)
    {
        var scene = new Scene(name);

        MainWindow.OnRender += scene.OnRender;
        MainWindow.OnUpdate += scene.OnUpdate;
        
        _scenes.Add(scene);

        ActiveSceneId ??= scene.Id;

        return scene;
    }

    public void Run()
    {
        AppStart = DateTime.Now;
        MainWindow.Run();
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

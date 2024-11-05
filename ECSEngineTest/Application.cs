using ECSEngineTest.Helpers;
using Silk.NET.OpenGL;
using System.Drawing;

namespace ECSEngineTest;

public class Application : IDisposable
{
    private readonly SortedList<int, Layer> _layers = [];
    private readonly List<Scene> _scenes = [];
    private readonly Window _mainWindow;

    private Scene? _activeScene = null;

    public static DateTime AppStart { get; private set; }

    public IReadOnlyList<Scene> Scenes => _scenes;
    public Scene? ActiveScene
    {
        get => _activeScene;
        set
        {
            if (value is null) return;

            _layers[0] = _activeScene = value;
        }
    }

    public event Action<Application> Init;

    public Application(WindowSettings windowSettings)
    {
        _mainWindow = new(windowSettings);
        _mainWindow.OnLoad += OnMainWindowLoad;
        _mainWindow.OnClosing += OnMainWindowClosing;
        _mainWindow.OnFileDrop += OnMainWindowFileDrop;
        _mainWindow.OnUpdate += OnMainWindowUpdate;
        _mainWindow.OnRender += OnMainWindowRender;
    }

    private void OnMainWindowLoad()
    {
        CreateEditor();

        Window.GL.Enable(EnableCap.DepthTest);
        Window.GL.Enable(EnableCap.CullFace);
        Window.GL.CullFace(TriangleFace.Back);
        Window.GL.FrontFace(FrontFaceDirection.Ccw);

        Window.GL.ClearColor(Color.FromArgb(222, 235, 255));

        ShaderUniforms.InitUniformBlocks();

        Init?.Invoke(this);
    }

    private void CreateEditor()
    {
        _layers[1] = new EditorLayer("ImGui Editor", _mainWindow.CreateImGui());
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

    private void OnMainWindowUpdate(double deltaTime)
    {
        var args = new LayerEventArgs(deltaTime);
        for (int i = _layers.Count - 1; i >= 0; i--)
        {
            var layer = _layers[i];
            if (layer is null || !layer.Enabled) continue;

            layer.OnUpdate(this, args);

            if (args.Cancel) break;
        }
    }

    private void OnMainWindowRender(double deltaTime)
    {
        var args = new LayerEventArgs(deltaTime);
        for (int i = 0; i < _layers.Count; i++)
        {
            var layer = _layers[i];
            if (layer is null || !layer.Enabled) continue;

            layer.OnRender(this, args);

            if (args.Cancel) break;
        }
    }

    public Scene CreateScene(string name)
    {
        var scene = new Scene(name);

        _scenes.Add(scene);

        ActiveScene ??= scene;

        return scene;
    }

    public void DeleteScene(uint sceneId)
    {
        var scene = _scenes.FirstOrDefault(x => x.Id == sceneId);
        if (scene is null) return;

        scene.Dispose();
        _scenes.Remove(scene);

        if (ActiveScene == scene)
        {
            if (_scenes.Count > 0)
            {
                _layers[0] = ActiveScene = _scenes[0];
            }
            else
            {
                ActiveScene = null;
                _layers.Remove(0);
            }
        }
    }

    public void Run()
    {
        AppStart = DateTime.Now;
        _mainWindow.Run();
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

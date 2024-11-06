using Silk.NET.OpenGL;
using System.Drawing;

namespace ECSEngineTest;

public class Application : IDisposable
{
    private readonly SortedList<int, Layer?> _layers = [];
    private readonly List<Scene> _scenes = [];
    private readonly Window _mainWindow;

    public static DateTime AppStart { get; private set; }

    public IReadOnlyList<Scene> Scenes => _scenes;
    public Scene? ActiveScene
    {
        get => _scenes.FirstOrDefault(x => x.Enabled);
        set
        {
            var currentScene =  _scenes.FirstOrDefault(x => x.Id == value?.Id);
            if (currentScene != null)
                currentScene.Enabled = false;

            if (value != null)
                value.Enabled = true;

            _layers[0] = value;
        }
    }

    public event Action<Application> Init;

    public Application(WindowSettings windowSettings)
    {
        _mainWindow = new(windowSettings);
        _mainWindow.OnUpdate += OnMainWindowUpdate;
        _mainWindow.OnRender += OnMainWindowRender;

        EventManager.WindowLoaded += OnMainWindowLoaded;
        EventManager.WindowClosing += OnMainWindowClosing;
    }

    private void OnMainWindowLoaded(object? sender, WindowLoadedEventArgs e)
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

    private void OnMainWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        ShaderManager.Dispose();
        ShaderUniforms.Dispose();
        MeshManager.DisposeAllMeshes();
    }

    private void OnMainWindowUpdate(double deltaTime)
    {
        var args = new LayerEventArgs(deltaTime);
        for (int i = _layers.Count - 1; i >= 0; i--)
        {
            if (!_layers.TryGetValue(i, out var layer) || layer is null || !layer.Enabled)
                continue;

            layer.OnUpdate(this, args);

            if (args.Cancel) break;
        }
    }

    private void OnMainWindowRender(double deltaTime)
    {
        var args = new LayerEventArgs(deltaTime);
        for (int i = 0; i < _layers.Count; i++)
        {
            if (!_layers.TryGetValue(i, out var layer) || layer is null || !layer.Enabled)
                continue;

            layer.OnRender(this, args);

            if (args.Cancel) break;
        }
    }

    public Scene CreateScene(string name)
    {
        var scene = new Scene(name);

        ActiveScene ??= scene;
        _scenes.Add(scene);

        return scene;
    }

    private void CreateEditor()
    {
        _layers[1] = new Editor("ImGui Editor", _mainWindow.CreateImGui());
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

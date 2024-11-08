using Silk.NET.OpenGL;
using System.Drawing;

namespace ECSEngineTest;

public class Application : IDisposable
{
    private readonly Window _mainWindow;

    public static DateTime AppStart { get; private set; }

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
        LayerManager.Update(deltaTime);
    }

    private void OnMainWindowRender(double deltaTime)
    {
        LayerManager.Render(deltaTime);
    }

    public Scene CreateScene(string name)
    {
        return SceneManager.CreateScene(name);
    }

    private void CreateEditor()
    {
        _layers[1] = new Editor("ImGui Editor", _mainWindow.CreateImGui());
    }

    public void DeleteScene(Scene scene)
    {
        SceneManager.DeleteScene(scene);
    }

    public void Run()
    {
        AppStart = DateTime.Now;
        _mainWindow.Run();
    }

    public void Dispose()
    {
        SceneManager.Dispose();
        Window.GL.Dispose();
    }
}

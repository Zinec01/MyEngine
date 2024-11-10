using Silk.NET.OpenGL;
using System.Drawing;

namespace ECSEngineTest;

public class Application : IDisposable
{
    public Window Window { get; }

    public static DateTime AppStart { get; private set; }

    public event Action<Application> Init;

    public Application(WindowSettings windowSettings)
    {
        Window = new(windowSettings);
        Window.OnUpdate += LayerManager.Update;
        Window.OnRender += LayerManager.Render;

        EventManager.WindowLoaded += OnWindowLoaded;
        EventManager.WindowClosing += OnWindowClosing;
    }

    public void AddImGuiOverlay()
    {
        LayerManager.AddOverlay(new ImGuiOverlay("ImGuiOverlay", Window.CreateImGuiController()));
    }

    private void OnWindowLoaded(object? sender, WindowLoadedEventArgs e)
    {
        Window.GL.Enable(EnableCap.DepthTest);
        Window.GL.Enable(EnableCap.CullFace);
        Window.GL.CullFace(TriangleFace.Back);
        Window.GL.FrontFace(FrontFaceDirection.Ccw);

        Window.GL.ClearColor(Color.FromArgb(222, 235, 255));

        ShaderUniforms.InitUniformBlocks();

        Init?.Invoke(this);
    }

    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        ShaderManager.Dispose();
        ShaderUniforms.Dispose();
        MeshManager.DisposeAllMeshes();
    }

    public void Run()
    {
        AppStart = DateTime.Now;
        Window.Run();
    }

    public void Dispose()
    {
        SceneManager.Dispose();
        Window.GL.Dispose();
    }
}

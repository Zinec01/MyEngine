using ECSEngineTest.Input;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Drawing;
using System.Numerics;

namespace ECSEngineTest;

public class Window
{
    private readonly IWindow _window;
    private IInputContext _inputContext;
    internal static GL GL { get; private set; }

    public Color BackgroundColor { get; set; } = Color.FromArgb(222, 235, 255);
    public string Title { get => _window.Title; set => _window.Title = value; }
    public WindowState State => (WindowState)(int)_window.WindowState;
    public Vector2 Size => new Vector2(_window.Size.X, _window.Size.Y);
    public Vector2 FrameBufferSize => new Vector2(_window.FramebufferSize.X, _window.FramebufferSize.Y);
    public Vector2 Position => new Vector2(_window.Position.X, _window.Position.Y);
    public bool VSync { get => _window.VSync; set => _window.VSync = value; }
    public double Time => _window.Time;

    internal event Action<double, double> OnUpdate;
    internal event Action<double, double> OnRender;

    public Window(WindowSettings settings)
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(settings.Width, settings.Height);
        options.Title = settings.Title;
        options.VSync = settings.VSync;

        _window = Silk.NET.Windowing.Window.Create(options);

        _window.Load += OnWindowLoad;
        _window.Update += OnWindowUpdate;
        _window.Render += OnWindowRender;
        _window.FramebufferResize += OnWindowFramebufferResize;
        _window.Closing += OnWindowClosing;
        _window.FileDrop += OnWindowFileDrop;
    }

    public void Run()
    {
        _window.Run();
        _window.Dispose();
    }

    public void Close()
    {
        _window.Close();
    }

    public ImGuiController CreateImGuiController()
    {
        return new ImGuiController(GL, _window, _inputContext);
    }

    private void OnWindowLoad()
    {
        _inputContext = _window.CreateInput();
        GL ??= GL.GetApi(_window);
        _window.Center();

        InputManager.Init(_inputContext);

        EventManager.RaiseEvent(EventTypeFlags.WindowLoaded, new EventRaiseDto { Sender = this, Window = this });
    }

    private void OnWindowUpdate(double dt)
    {
        OnUpdate?.Invoke(dt, _window.Time);
        MainThreadDispatcher.ExecuteOnMainThread();
    }

    private void OnWindowRender(double dt)
    {
        OnRender?.Invoke(dt, _window.Time);
    }

    private void OnWindowFramebufferResize(Vector2D<int> newSize)
    {
        EventManager.RaiseEvent(EventTypeFlags.WindowResized, new EventRaiseDto { Sender = this, Window = this });
    }

    private void OnWindowFileDrop(string[] filePaths)
    {
        EventManager.RaiseEvent(EventTypeFlags.WindowFileDrop, new EventRaiseDto { Sender = this, Window = this, FilePaths = filePaths });
    }

    private void OnWindowClosing()
    {
        EventManager.RaiseEvent(EventTypeFlags.WindowClosing, new EventRaiseDto { Sender = this, Window = this });
    }
}

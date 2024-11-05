using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Numerics;

namespace ECSEngineTest;

public class Window
{
    private readonly IWindow _window;
    private IInputContext _inputContext;
    internal static GL GL { get; private set; }

    public WindowState State => (WindowState)(int)_window.WindowState;

    public event Action OnLoad;
    public event Action<Vector2> OnResize;
    public event Action<string[]> OnFileDrop;
    public event Action OnClosing;

    internal event Action<double> OnUpdate;
    internal event Action<double> OnRender;

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
        // TODO: WindowDisposing Event
        _window.Dispose();
    }

    public void Close()
    {
        _window.Close();
    }

    public ImGuiController CreateImGui()
    {
        return new ImGuiController(GL, _window, _inputContext);
    }

    private void OnWindowLoad()
    {
        _inputContext = _window.CreateInput();
        GL = GL.GetApi(_window);
        _window.Center();

        foreach (var keyboard in _inputContext.Keyboards)
        {
            keyboard.KeyDown += (sender, key, code) =>
            {
                if (key == Key.Escape)
                {
                    Close();
                    return;
                }

                //EventManager.RaiseEvent(EventTypeFlags.KeyboardEvent, this, sender);
            };
        }

        foreach (var mouse in _inputContext.Mice)
        {
            mouse.MouseDown += (sender, button) => EventManager.RaiseEvent(EventTypeFlags.MouseDown, new EventRaiseDto { Sender = this, Data = [(Input.MouseButton)(int)button] });
            mouse.MouseUp += (sender, button) => EventManager.RaiseEvent(EventTypeFlags.MouseUp, new EventRaiseDto { Sender = this, Data = [(Input.MouseButton)(int)button] });
        }

        _inputContext.ConnectionChanged += (sender, args) =>
        {
            //EventManager.RaiseEvent(EventTypeFlags.InputConnectionChanged, this, args);
        };

        _window.FileDrop += (files) =>
        {
        };

        OnLoad?.Invoke();
    }

    private void OnWindowUpdate(double dt)
    {
        OnUpdate?.Invoke(dt);
        MainThreadDispatcher.ExecuteOnMainThread();
    }

    private void OnWindowRender(double dt)
    {
        OnRender?.Invoke(dt);
    }

    private void OnWindowFramebufferResize(Vector2D<int> newSize)
    {
        OnResize?.Invoke(new Vector2(newSize.X, newSize.Y));
    }

    private void OnWindowFileDrop(string[] fileNames)
    {
        OnFileDrop?.Invoke(fileNames);
    }

    private void OnWindowClosing()
    {
        OnClosing?.Invoke();
    }
}

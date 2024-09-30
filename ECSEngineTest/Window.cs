using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace ECSEngineTest;

public class Window
{
    private readonly IWindow _window;
    private IInputContext InputContext { get; set; }
    internal static GL GL { get; private set; }

    public WindowState State => _window.WindowState;

    public event EventHandler OnLoad;

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
        _window.Closing += OnWindowClose;
        //_window.Initialize();
    }

    public void Run()
    {
        _window.Run();
        // TODO: WindowDisposing Event
        _window.Dispose();
    }

    private void OnWindowLoad()
    {
        InputContext = _window.CreateInput();
        GL = GL.GetApi(_window);
        _window.Center();

        foreach (var keyboard in InputContext.Keyboards)
        {
            keyboard.KeyDown += (sender, key, code) =>
            {
                if (key == Key.Escape) _window.Close();
            };
        }

        //TODO: WindowClosed Event
        OnLoad?.Invoke(this, EventArgs.Empty);
    }

    private void OnWindowUpdate(double dt)
    {
        //TODO: Update Event
    }

    private void OnWindowRender(double dt)
    {
        //TODO: Render Event
    }

    private void OnWindowFramebufferResize(Vector2D<int> newSize)
    {
        //TODO: WindowResized Event
    }

    private void OnWindowClose()
    {
        //TODO: WindowClosing Event
        ShaderManager.Dispose();
    }
}

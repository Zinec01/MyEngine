using Silk.NET.OpenGL.Extensions.ImGui;
using System.Numerics;

namespace ECSEngineTest;

internal class Editor : Layer
{
    private readonly ImGuiController _imGuiController;
    private const float _fpsRefreshRate = 0.5f;
    private double _elapsedTime = 0.0;
    private int _fps = 0;

    public Editor(string name, ImGuiController imGuiController) : base(name, 1)
    {
        _imGuiController = imGuiController;

        EventManager.MouseDown += OnMouseDown;
        EventManager.MouseUp += OnMouseUp;
        EventManager.MouseClick += OnMouseClick;
        EventManager.MouseDoubleClick += OnMouseDoubleClick;
        EventManager.MouseScroll += OnMouseScroll;
        EventManager.KeyDown += OnKeyDown;
        EventManager.KeyUp += OnKeyUp;
        EventManager.WindowFileDrop += OnFileDrop;
    }

    internal override void OnUpdate(object? sender, LayerEventArgs args)
    {
        _elapsedTime += args.DeltaTime;

        if (_elapsedTime >= _fpsRefreshRate)
        {
            _fps = (int)(1.0 / args.DeltaTime);
            _elapsedTime = 0.0;
        }

        _imGuiController.Update((float)args.DeltaTime);
    }

    internal override void OnRender(object? sender, LayerEventArgs args)
    {
        if (ImGuiNET.ImGui.Begin("Info", ImGuiNET.ImGuiWindowFlags.NoResize
                                         | ImGuiNET.ImGuiWindowFlags.NoMove
                                         | ImGuiNET.ImGuiWindowFlags.NoCollapse))
        {
            ImGuiNET.ImGui.SetWindowPos(Vector2.Zero);
            ImGuiNET.ImGui.SetWindowSize(new Vector2(90, 55));

            if (ImGuiNET.ImGui.BeginTable("FPS", 2))
            {
                ImGuiNET.ImGui.TableNextColumn();
                ImGuiNET.ImGui.Text("FPS");
                ImGuiNET.ImGui.TableNextColumn();
                ImGuiNET.ImGui.Text(_fps.ToString());
                ImGuiNET.ImGui.EndTable();
            }

            ImGuiNET.ImGui.End();
        }

        //ImGuiNET.ImGui.ShowDemoWindow();

        _imGuiController.Render();
    }

    public void OnMouseDown(object? sender, MouseDownEventArgs e)
    {
        Console.WriteLine($"Editor:\tMouse Down - {e.Buttons.Select(x => x.ToString()).Aggregate((c, n) => c + ", " + n)}");

        e.Cancel = ImGuiNET.ImGui.GetIO().WantCaptureMouse;
    }

    public void OnMouseUp(object? sender, MouseUpEventArgs e)
    {
        Console.WriteLine($"Editor:\tMouse Up - {e.Button}");

        e.Cancel = ImGuiNET.ImGui.GetIO().WantCaptureMouse;
    }

    private void OnMouseClick(object? sender, MouseClickEventArgs e)
    {
        Console.WriteLine($"Editor:\tMouse Click - {e.Button}");

        e.Cancel = ImGuiNET.ImGui.GetIO().WantCaptureMouse;
    }

    private void OnMouseDoubleClick(object? sender, MouseClickEventArgs e)
    {
        Console.WriteLine($"Editor:\tMouse Double Click - {e.Button}");

        e.Cancel = ImGuiNET.ImGui.GetIO().WantCaptureMouse;
    }

    private void OnMouseScroll(object? sender, MouseScrollEventArgs e)
    {
        Console.WriteLine($"Editor:\tMouse Scroll - X: {e.X}, Y: {e.Y}");

        e.Cancel = ImGuiNET.ImGui.GetIO().WantCaptureMouse;
    }

    private void OnKeyDown(object? sender, KeyDownEventArgs e)
    {
        Console.WriteLine($"Editor:\tKey Down - {e.Keys.Select(x => x.ToString()).Aggregate((c, n) => c + ", " + n)}");

        e.Cancel = ImGuiNET.ImGui.GetIO().WantCaptureMouse;
    }

    private void OnKeyUp(object? sender, KeyUpEventArgs e)
    {
        Console.WriteLine($"Editor:\tKey Down - {e.Key}");

        e.Cancel = ImGuiNET.ImGui.GetIO().WantCaptureMouse;
    }

    private void OnFileDrop(object? sender, WindowFileDropEventArgs e)
    {
        Console.WriteLine($"Editor:\tFile Drop - {e.FilePaths.Select(x => Path.GetFileName(x)).Aggregate((c, n) => c + ", " + n)}");

        e.Cancel = ImGuiNET.ImGui.GetIO().WantCaptureMouse;
    }
}

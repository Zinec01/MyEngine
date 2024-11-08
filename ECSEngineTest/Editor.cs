using Friflo.Engine.ECS;
using Silk.NET.OpenGL.Extensions.ImGui;
using System.Numerics;

namespace ECSEngineTest;

internal class Editor : Layer
{
    private readonly ImGuiController _imGuiController;
    private float _refreshRate = 0.25f;
    private double _elapsedUpdateTime = 0.0;
    private double _elapsedRenderTime = 0.0;
    private int _fps = 0;
    private double _updateDeltaTime = 0.0;
    private double _renderDeltaTime = 0.0;

    private static EntityStore? Store => SceneManager.ActiveScene?.EntityStore;

    public Editor(string name, ImGuiController imGuiController) : base(name, int.MaxValue)
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

    internal override void OnUpdate(LayerEventArgs args)
    {
        _elapsedUpdateTime += args.DeltaTime;

        if (_elapsedUpdateTime >= _refreshRate)
        {
            _updateDeltaTime = args.DeltaTime * 1000;

            _elapsedUpdateTime = 0.0;
        }

        _imGuiController.Update((float)args.DeltaTime);
    }

    internal override void OnRender(LayerEventArgs args)
    {
        _elapsedRenderTime += args.DeltaTime;

        if (_elapsedRenderTime >= _refreshRate)
        {
            _fps = (int)(1.0 / (_elapsedUpdateTime + args.DeltaTime));
            _renderDeltaTime = args.DeltaTime * 1000;

            _elapsedRenderTime = 0.0;
        }

        CreateInfoWindow(Vector2.Zero);
        CreateEntityListWindow();

        //ImGuiNET.ImGui.ShowDemoWindow();

        _imGuiController.Render();
    }

    private void CreateInfoWindow(Vector2 position, Vector2? size = null)
    {
        var flags = ImGuiNET.ImGuiWindowFlags.NoResize
                    | ImGuiNET.ImGuiWindowFlags.NoMove
                    | ImGuiNET.ImGuiWindowFlags.NoCollapse;

        if (!size.HasValue)
            flags |= ImGuiNET.ImGuiWindowFlags.AlwaysAutoResize;

        if (ImGuiNET.ImGui.Begin("Info", flags))
        {
            ImGuiNET.ImGui.SetWindowPos(position);

            if (size.HasValue)
                ImGuiNET.ImGui.SetWindowSize(size.Value);

            ImGuiNET.ImGui.SliderFloat("Refresh rate (s)", ref _refreshRate, 0.01f, 1.0f, "%.2f");

            if (ImGuiNET.ImGui.BeginTable("Info", 2))
            {
                ImGuiNET.ImGui.TableNextColumn();
                ImGuiNET.ImGui.Text("Update time");

                ImGuiNET.ImGui.TableNextColumn();
                ImGuiNET.ImGui.Text($"{_updateDeltaTime:0.0000} ms");

                ImGuiNET.ImGui.TableNextColumn();
                ImGuiNET.ImGui.Text("Render time");

                ImGuiNET.ImGui.TableNextColumn();
                ImGuiNET.ImGui.Text($"{_renderDeltaTime:0.0000} ms");

                ImGuiNET.ImGui.TableNextColumn();
                ImGuiNET.ImGui.Text("FPS");

                ImGuiNET.ImGui.TableNextColumn();
                ImGuiNET.ImGui.Text(_fps.ToString());

                ImGuiNET.ImGui.EndTable();
            }

            ImGuiNET.ImGui.End();
        }
    }

    private void CreateEntityListWindow()
    {

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

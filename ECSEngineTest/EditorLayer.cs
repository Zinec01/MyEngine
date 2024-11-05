using Silk.NET.OpenGL.Extensions.ImGui;

namespace ECSEngineTest;

internal class EditorLayer : Layer
{
    private readonly ImGuiController _imGuiController;
    private int _fps = 0;

    private bool _mouseDown = false;

    public EditorLayer(string name, ImGuiController imGuiController) : base(name, 1)
    {
        _imGuiController = imGuiController;

        EventManager.MouseDown += (sender, args) =>
        {
            if (ImGuiNET.ImGui.IsAnyItemHovered())
            {
                args.Cancel = true;
            }

            _mouseDown = true;
            Task.Run(() =>
            {
                while (_mouseDown)
                {
                    Console.WriteLine($"{DateTime.Now.Millisecond} Mouse Down");
                }
            });
        };

        EventManager.MouseUp += (sender, args) =>
        {
            _mouseDown = false;
            Console.WriteLine($"{DateTime.Now.Millisecond} Mouse Up");
        };
    }

    internal override void OnUpdate(object? sender, LayerEventArgs args)
    {
        _fps = (int)(1 / args.DeltaTime);
        _imGuiController.Update((float)args.DeltaTime);
    }

    internal override void OnRender(object? sender, LayerEventArgs args)
    {
        if (ImGuiNET.ImGui.Begin("Info"))
        {
            ImGuiNET.ImGui.DragInt("FPS", ref _fps, 0, 0, 0, "%d", ImGuiNET.ImGuiSliderFlags.NoInput);
            ImGuiNET.ImGui.End();
        }

        _imGuiController.Render();
    }
}

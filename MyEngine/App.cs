using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.OpenGL;
using System.Numerics;
using Silk.NET.Windowing;

namespace MyEngine;

internal class App
{
    public static GL GL { get; private set; }
    private IWindow Window { get; set; }
    private IInputContext InputContext { get; set; }
    private ImGuiController ImGuiController { get; set; }

    public static int FPS { get; private set; }
    private static bool VSync;
    private static DateTime Started { get; set; }

    private static Scene Scene { get; set; }

    public App(int width, int height, string name)
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(width, height);
        options.Title = name;
        options.VSync = true;

        Window = Silk.NET.Windowing.Window.Create(options);

        Window.Load += OnLoad;
        Window.Update += OnUpdate;
        Window.Render += OnRender;
        Window.FramebufferResize += OnFramebufferResize;
        Window.Closing += OnClose;
    }

    public void Run()
    {
        Window.Run();
        Window.Dispose();
    }

    private void OnLoad()
    {
        InputContext = Window.CreateInput();
        foreach (var keyboard in InputContext.Keyboards)
        {
            keyboard.KeyDown += OnKeyDown;
        }
        foreach (var mouse in InputContext.Mice)
        {
            mouse.DoubleClickTime = 0;
            mouse.Click += OnMouseClick;
        }
        Window.Center();

        GL = GL.GetApi(Window);

        //Console.WriteLine("VSync " + (Window.VSync ? "ON" : "OFF"));

        ImGuiController = new ImGuiController(GL, Window, InputContext);

        Started = DateTime.Now;
        VSync = Window.VSync;

        var triangleVerts = new[]
        {
            -1f, -1f, 0f,   0f, 1f,
             0f,  1f, 0f, 0.5f, 0f,
             1f, -1f, 0f,   1f, 1f
        };

        var triangleInds = new[] { 0, 1, 2 };

        var pyramidVerts = new[]
        {
             0f,  1f,  0f, 0.5f, 0f,
            -1f, -1f,  1f,   1f, 1f,
             1f, -1f,  1f,   0f, 1f,

             0f,  1f,  0f, 0.5f, 0f,
             1f, -1f,  1f,   1f, 1f,
             1f, -1f, -1f,   0f, 1f,

             0f,  1f,  0f, 0.5f, 0f,
             1f, -1f, -1f,   1f, 1f,
            -1f, -1f, -1f,   0f, 1f,

             0f,  1f,  0f, 0.5f, 0f,
            -1f, -1f, -1f,   1f, 1f,
            -1f, -1f,  1f,   0f, 1f,

            -1f, -1f, -1f,   0f, 0f,
             1f, -1f, -1f,   1f, 0f,
            -1f, -1f,  1f,   0f, 1f,

            -1f, -1f,  1f,   0f, 1f,
             1f, -1f, -1f,   1f, 0f,
             1f, -1f,  1f,   1f, 1f
        };

        var pyramidInds = new[]
        {
             0,  1,  2,
             3,  4,  5,
             6,  7,  8,
             9, 10, 11,
            12, 13, 14,
            15, 16, 17
        };

        var squareVerts = new[]
        {
             1f,  1f, 0f, 1f, 0f,
             1f, -1f, 0f, 1f, 1f,
            -1f, -1f, 0f, 0f, 1f,
            -1f,  1f, 0f, 0f, 0f
        };

        var squareInds = new[]
        {
            0, 1, 3,
            1, 2, 3
        };

        EventHandler<float> transformAction = (sender, deltaTime) =>
        {
            if (sender is not Model model) return;

            model.Transform.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)deltaTime / 2));
            model.Transform.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)deltaTime / 2));
            model.Transform.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)deltaTime / 2));

            if (InputContext.Keyboards[0].IsKeyPressed(Key.Left))
            {
                model.Transform.Move(new Vector3((float)-deltaTime, 0, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Right))
            {
                model.Transform.Move(new Vector3((float)deltaTime, 0, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Up))
            {
                model.Transform.Move(new Vector3(0, (float)deltaTime, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Down))
            {
                model.Transform.Move(new Vector3(0, (float)-deltaTime, 0));
            }
        };

        var pyramid = new Model(pyramidVerts, pyramidInds, @"..\..\..\Textures\obama.jpg");
        pyramid.Transform.SetScale(0.25f);
        pyramid.Transform.SetPosition(new Vector3(0.5f, 0, 0));
        //pyramid.Transform.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, 1f));

        pyramid.OnPermanentTransform += transformAction;

        var triangle = new Model(triangleVerts, triangleInds, @"..\..\..\Textures\obama.jpg");
        triangle.Transform.SetScale(0.25f);
        triangle.Transform.SetPosition(new Vector3(-0.5f, 0, 0));
        triangle.OnPermanentTransform += transformAction;

        var square = new Model(squareVerts, squareInds, @"..\..\..\Textures\obama.jpg");
        square.Transform.SetScale(0.25f);
        square.OnPermanentTransform += transformAction;

        Scene = new Scene();
        Scene.TryAddModel(1, pyramid);
        Scene.TryAddModel(2, triangle);
        Scene.TryAddModel(3, square);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.StencilTest);
        GL.CullFace(TriangleFace.Front);
        //GL.FrontFace(FrontFaceDirection.CW);
    }

    private static void OnMouseClick(IMouse mouse, MouseButton button, Vector2 position)
    {
        //var pos = Scene.Objects.First().Value.Transform.CurrentPosition;
        //GL.ReadPixels<int>((int)pos.X, (int)pos.Y, (uint)Window.FramebufferSize.X, (uint)Window.FramebufferSize.Y, PixelFormat.DepthStencil, PixelType.Int, out var pixels);
    }

    private void OnUpdate(double deltaTime)
    {
        if (DateTime.Now.Millisecond % 250 < 10)
        {
            FPS = (int)(1 / deltaTime);
            //Console.WriteLine($"{FPS} FPS");

            if (Window.VSync != VSync)
            {
                Window.VSync = VSync;
                Thread.Sleep(20);
            }
        }

        Scene.UpdateObjects(deltaTime);
    }

    private void OnRender(double deltaTime)
    {
        ImGuiController.Update((float)deltaTime);

        GL.ClearColor(0.07f, 0.01f, 0.02f, 1f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

        if (ImGuiNET.ImGui.Begin("Info"))
        {
            ImGuiNET.ImGui.Text($"{FPS} FPS");
            ImGuiNET.ImGui.Checkbox("VSync", ref VSync);
        }

        Scene.Draw();

        ImGuiController.Render();
    }

    private static void OnFramebufferResize(Vector2D<int> newSize)
    {
        GL.Viewport(newSize);
        Console.WriteLine($"New resolution: {newSize.X}x{newSize.Y}");
    }

    private void OnKeyDown(IKeyboard keyboard, Key key, int arg3)
    {
        switch (key)
        {
            case Key.Escape:
                Window.Close();
                break;
        }
    }

    private static void OnClose()
    {
        Console.WriteLine("gg");

        Scene.Dispose();
        GL.Dispose();
    }
}

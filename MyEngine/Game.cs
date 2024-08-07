using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.OpenGL;
using System.Numerics;
using Silk.NET.Windowing;

namespace MyEngine;

internal class Game
{
    public static GL GL { get; private set; }
    public IWindow Window { get; private set; }
    private IInputContext InputContext { get; set; }
    private ImGuiController ImGuiController { get; set; }

    public static int FPS { get; private set; }
    private static bool VSync;
    public static DateTime Start { get; private set; }

    private List<Scene> Scenes { get; } = [];
    private int _activeSceneID = 0;

    private Scene? ActiveScene => Scenes.FirstOrDefault(x => x.Id == _activeSceneID);

    public static event EventHandler<byte> MouseScroll;
    public static event EventHandler<Vector2> MouseClick;
    public static event EventHandler<Vector2> MouseMove;
    public static event EventHandler<Key> KeyDown;


    public Game(int width, int height, string name)
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
        Console.WriteLine("Hello there");

        InputContext = Window.CreateInput();
        foreach (var keyboard in InputContext.Keyboards)
        {
            keyboard.KeyDown += OnKeyDown;
        }
        foreach (var mouse in InputContext.Mice)
        {
            mouse.DoubleClickTime = 0;
            mouse.Click += OnMouseClick;
            mouse.MouseMove += OnMouseMove;
            mouse.Scroll += OnMouseScroll;
        }
        Window.Center();

        GL = GL.GetApi(Window);

        //Console.WriteLine("VSync " + (Window.VSync ? "ON" : "OFF"));

        ImGuiController = new ImGuiController(GL, Window, InputContext);

        Start = DateTime.Now;
        VSync = Window.VSync;

        var triangleVerts = new[]
        {
             0f,  1f, 0f, 0.5f, 0f,
             1f, -1f, 0f,   1f, 1f,
            -1f, -1f, 0f,   0f, 1f
        };

        var triangleInds = new[] { 0, 1, 2 };

        var pyramidVerts = new[]
        {
             0f,  1f,  0f, 0.5f, 0f,
            -1f, -1f,  1f,   1f, 1f,
             1f, -1f,  1f,   0f, 1f,
             1f, -1f, -1f,   0f, 1f,
             1f, -1f, -1f,   1f, 1f,
            -1f, -1f, -1f,   0f, 1f,
            -1f, -1f, -1f,   1f, 1f,
            -1f, -1f,  1f,   0f, 1f,
            -1f, -1f, -1f,   0f, 0f,
             1f, -1f, -1f,   1f, 0f,
             1f, -1f,  1f,   1f, 1f
        };

        var pyramidInds = new[]
        {
             0, 1, 2,
             0, 1, 3,
             0, 4, 5,
             0, 6, 7,
             8, 9, 7,
             7, 9, 10
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

            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)deltaTime / 2));
            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)deltaTime / 2));
            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)deltaTime / 2));

            if (InputContext.Keyboards[0].IsKeyPressed(Key.Left))
            {
                model.Move(new Vector3((float)-deltaTime, 0, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Right))
            {
                model.Move(new Vector3((float)deltaTime, 0, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Up))
            {
                model.Move(new Vector3(0, (float)deltaTime, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Down))
            {
                model.Move(new Vector3(0, (float)-deltaTime, 0));
            }
        };

        var pyramid = new Model(pyramidVerts, pyramidInds, @"..\..\..\Textures\obama.jpg");
        pyramid.SetScale(0.25f);
        pyramid.SetPosition(new Vector3(0.5f, 0, 0));
        //pyramid.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, 1f));

        pyramid.OnPermanentTransform += transformAction;

        var triangle = new Model(triangleVerts, triangleInds, @"..\..\..\Textures\obama.jpg");
        triangle.SetScale(0.25f);
        triangle.SetPosition(new Vector3(-0.5f, 0, 0));
        triangle.OnPermanentTransform += transformAction;

        var square = new Model(squareVerts, squareInds, @"..\..\..\Textures\obama.jpg");
        square.SetScale(0.25f);
        square.OnPermanentTransform += transformAction;

        var scene = new Scene();
        scene.Objects.Add(pyramid);
        scene.Objects.Add(triangle);
        scene.Objects.Add(square);

        Scenes.Add(scene);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        //GL.Enable(EnableCap.StencilTest);
        GL.CullFace(TriangleFace.Front);
        //GL.FrontFace(FrontFaceDirection.CW);

        Console.WriteLine($"Rendering {Scenes.Select(x => x.Objects.Count).Sum()} objects in total of {Scenes.Count} scenes");
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

        ActiveScene?.UpdateObjects((float)deltaTime);
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

        ActiveScene?.Draw();

        ImGuiController.Render();
    }

    private static void OnFramebufferResize(Vector2D<int> newSize)
    {
        GL.Viewport(newSize);
        Console.WriteLine($"New resolution: {newSize.X}x{newSize.Y}");
    }


    #region Input handling

    private void OnMouseMove(IMouse mouse, Vector2 position)
    {
        MouseMove?.Invoke(mouse, position);
    }

    private void OnMouseScroll(IMouse mouse, ScrollWheel args)
    {
        Console.WriteLine($"Mouse scroll {args.X}x{args.Y}");
        MouseScroll?.Invoke(mouse, (byte)args.Y);
    }

    private void OnMouseClick(IMouse mouse, MouseButton button, Vector2 position)
    {
        //var pos = Scene.Objects.First().Value.CurrentPosition;
        //GL.ReadPixels<int>((int)pos.X, (int)pos.Y, (uint)Window.FramebufferSize.X, (uint)Window.FramebufferSize.Y, PixelFormat.DepthStencil, PixelType.Int, out var pixels);

        MouseClick?.Invoke(mouse, position);
    }

    private void OnKeyDown(IKeyboard keyboard, Key key, int code)
    {
        switch (key)
        {
            case Key.Escape:
                Window.Close();
                break;
        }

        KeyDown?.Invoke(keyboard, key);
    }

    #endregion

    private void OnClose()
    {
        Console.WriteLine("GG");

        foreach (var scene in Scenes)
        {
            scene.Dispose();
        }

        GL.Dispose();
    }
}

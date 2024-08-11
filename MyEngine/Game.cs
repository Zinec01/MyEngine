using MyEngine.Interfaces;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Drawing;
using System.Numerics;

namespace MyEngine;

internal class Game
{
    public Color BackgroundColor { get; set; }

    private GL GL { get; set; }
    public IWindow Window { get; private set; }
    private IInputContext InputContext { get; set; }
    private ImGuiController ImGuiController { get; set; }

    private bool VSync;
    public int FPS { get; private set; }
    public DateTime Start { get; private set; }

    private uint _activeSceneId = 0;
    private List<Scene> Scenes { get; } = [];

    public Scene? ActiveScene
    {
        get => Scenes.FirstOrDefault(x => x.Id == _activeSceneId);
        set
        {
            if (value != null)
                _activeSceneId = value.Id;
        }
    }

    public event EventHandler<byte> MouseScroll;
    public event EventHandler<Vector2> MouseClick;
    public event EventHandler<Vector2> MouseMove;
    public event EventHandler<Key> KeyDown;


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

        if (InputContext.Keyboards.Count == 0)
        {
            Console.WriteLine("Bro just get a keyboard ???");
            Environment.Exit(0);
        }
        if (InputContext.Mice.Count == 0)
        {
            Console.WriteLine("Bro just get a mouse ???");
            Environment.Exit(0);
        }

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

        ImGuiController = new ImGuiController(GL, Window, InputContext);

        BackgroundColor = Color.FromArgb(255, 18, 3, 5);

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
            -1f, -1f, -1f,   0f, 1f,
             1f, -1f, -1f,   1f, 1f,
             1f, -1f, -1f,   0f, 1f,
             1f, -1f,  1f,   1f, 1f,
             1f, -1f,  1f,   0f, 1f,
            -1f, -1f,  1f,   1f, 1f,
            -1f, -1f,  1f,   0f, 1f,
            -1f, -1f, -1f,   1f, 1f,
            -1f, -1f, -1f,   0f, 0f,
             1f, -1f, -1f,   1f, 0f
        };

        var pyramidInds = new[]
        {
             0, 1, 2,
             0, 3, 4,
             0, 5, 6,
             0, 7, 8,
             9, 7, 10,
             10, 7, 4
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
            if (sender is not GameObject model) return;

            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)deltaTime / 2));
            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)deltaTime / 2));
            model.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)deltaTime / 2));

            if (InputContext.Keyboards[0].IsKeyPressed(Key.Left))
            {
                model.MoveBy(new Vector3((float)-deltaTime, 0, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Right))
            {
                model.MoveBy(new Vector3((float)deltaTime, 0, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Up))
            {
                model.MoveBy(new Vector3(0, (float)deltaTime, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Down))
            {
                model.MoveBy(new Vector3(0, (float)-deltaTime, 0));
            }
        };

        var pyramid = new GameObject(GL, pyramidVerts, pyramidInds, @"..\..\..\Textures\obama.jpg");
        //pyramid.SetScale(0.25f);
        pyramid.SetPosition(new Vector3(3f, 2f, 0));
        //pyramid.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, 1f));
        pyramid.PermanentTransform += transformAction;

        var triangle = new GameObject(GL, triangleVerts, triangleInds, @"..\..\..\Textures\obama.jpg");
        //triangle.SetScale(0.25f);
        triangle.SetPosition(new Vector3(-3f, 2f, 0));
        triangle.PermanentTransform += transformAction;

        var square = new GameObject(GL, squareVerts, squareInds, @"..\..\..\Textures\obama.jpg")
        {
            Parent = triangle
        };
        //square.SetScale(0.25f);
        square.SetPosition(new Vector3(0f, 2f, 0f));
        //square.PermanentTransform += transformAction;
        square.ParentObjectChanged += (sender, changeAction) =>
        {
            var parent = (ITransformable)sender!;

            if (changeAction.HasFlag(ObjectChangedFlag.ROTATION))
                square.SetRotation(parent.CurrentRotation);

            if (changeAction.HasFlag(ObjectChangedFlag.POSITION))
                square.SetPosition(square.CurrentPosition + (parent.CurrentPosition - parent.PreviousPosition));
        };

        var floor = new GameObject(GL, squareVerts, squareInds, @"..\..\..\Textures\xd.png");
        floor.SetRotation(Quaternion.CreateFromAxisAngle(Vector3.UnitX, -90f.DegToRad()));
        floor.SetScale(10f);

        var scene = new Scene(GL, Window, InputContext);
        scene.Objects.Add(pyramid);
        scene.Objects.Add(triangle);
        scene.Objects.Add(square);
        scene.Objects.Add(floor);

        Scenes.Add(scene);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        //GL.Enable(EnableCap.StencilTest);
        GL.CullFace(TriangleFace.Back);
        GL.FrontFace(FrontFaceDirection.CW);

        Console.WriteLine($"Rendering {Scenes.Select(x => x.Objects.Count).Sum()} objects in total of {Scenes.Count} scenes");
    }

    private void OnUpdate(double deltaTime)
    {
        if (DateTime.Now.Millisecond % 250 < 10)
        {
            FPS = (int)(1 / deltaTime);

            if (Window.VSync != VSync)
            {
                Window.VSync = VSync;
                Thread.Sleep(20);
            }
        }

        ActiveScene?.Update((float)deltaTime);
    }

    private void OnRender(double deltaTime)
    {
        ImGuiController.Update((float)deltaTime);

        GL.ClearColor(BackgroundColor);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

        if (ImGuiNET.ImGui.Begin("Info"))
        {
            ImGuiNET.ImGui.Text($"{FPS} FPS");
            ImGuiNET.ImGui.Checkbox("VSync", ref VSync);
        }
        
        ActiveScene?.Draw();

        ImGuiController.Render();
    }

    private void OnFramebufferResize(Vector2D<int> newSize)
    {
        GL.Viewport(newSize);
        Console.WriteLine($"New resolution: {newSize.X}x{newSize.Y}");
    }


    public void MakeFullscreen()
    {
        Window.WindowState = WindowState.Fullscreen;
    }


    #region Input handling

    private void OnMouseMove(IMouse mouse, Vector2 position)
    {
        MouseMove?.Invoke(mouse, position);
    }

    private void OnMouseScroll(IMouse mouse, ScrollWheel args)
    {
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

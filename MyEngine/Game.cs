using MyEngine.EventArgs;
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
            if (sender is not GameObject obj) return;

            Console.WriteLine($"{obj.Name} - transformAction");

            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)deltaTime / 2));
            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)deltaTime / 2));
            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)deltaTime / 2));

            //if (InputContext.Keyboards[0].IsKeyPressed(Key.Left))
            //{
            //    obj.MoveBy(new Vector3((float)-deltaTime, 0, 0));
            //}
            //if (InputContext.Keyboards[0].IsKeyPressed(Key.Right))
            //{
            //    obj.MoveBy(new Vector3((float)deltaTime, 0, 0));
            //}
            //if (InputContext.Keyboards[0].IsKeyPressed(Key.Up))
            //{
            //    obj.MoveBy(new Vector3(0, (float)deltaTime, 0));
            //}
            //if (InputContext.Keyboards[0].IsKeyPressed(Key.Down))
            //{
            //    obj.MoveBy(new Vector3(0, (float)-deltaTime, 0));
            //}
        };

        var pyramid = new GameObject(GL, "pyramid", pyramidVerts, pyramidInds, @"..\..\..\Textures\obama.jpg");
        pyramid.SetPosition(new Vector3(3f, 2f, 0));
        pyramid.PermanentTransform += transformAction;

        var triangle = new GameObject(GL, "triangle", triangleVerts, triangleInds, @"..\..\..\Textures\obama.jpg");
        triangle.SetPosition(new Vector3(-3f, 2f, 0));
        triangle.PermanentTransform += transformAction;

        var square = new GameObject(GL, "square", squareVerts, squareInds, @"..\..\..\Textures\obama.jpg")
        {
            Parent = triangle
        };
        square.SetPosition(new Vector3(0f, 2f, 0f));

        EventHandler<ParentObjectChangedArgs> copyParentRotationAndMovement = (sender, args) =>
        {
            if (sender is not GameObject obj) return;

            Console.WriteLine($"{obj.Name} - copyParentRotationAndMovement");

            var parent = args.Parent;

            if (args.ChangeEvent.HasFlag(ObjectChangedFlag.ROTATION))
                obj.SetRotation(parent.CurrentRotation);

            if (args.ChangeEvent.HasFlag(ObjectChangedFlag.POSITION))
                obj.SetPosition(obj.CurrentPosition + (parent.CurrentPosition - parent.PreviousPosition));
        };

        square.ParentObjectChanged += copyParentRotationAndMovement;

        var floor = new GameObject(GL, "floor", squareVerts, squareInds, @"..\..\..\Textures\xd.png");
        floor.SetRotation(Quaternion.CreateFromAxisAngle(Vector3.UnitX, -90f.DegToRad()));
        floor.SetScale(10f);


        var pyramidSun = new GameObject(GL, "pyramidSun", pyramidVerts, pyramidInds);
        var pyramidPlanet = new GameObject(GL, "pyramidPlanet", pyramidVerts, pyramidInds) { Parent = pyramidSun };
        var pyramidMoon = new GameObject(GL, "pyramidMoon", pyramidVerts, pyramidInds) { Parent = pyramidPlanet };

        pyramidSun.SetPosition(new Vector3(3f, 5f, 0f));
        pyramidPlanet.SetPosition(new Vector3(5f, 5f, 0f));
        pyramidMoon.SetPosition(new Vector3(5.5f, 5f, 0f));

        pyramidSun.SetScale(0.5f);
        pyramidPlanet.SetScale(0.3f);
        pyramidMoon.SetScale(0.15f);


        EventHandler<float> rotateAroundSelf = (sender, deltaTime) =>
        {
            if (sender is not GameObject obj) return;

            Console.WriteLine($"{obj.Name} - rotateAroundSelf");

            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, deltaTime * (1 - obj.CurrentScale) * 5));
        };

        EventHandler<float> rotateAroundParent = (sender, deltaTime) =>
        {
            if (sender is not GameObject obj || obj.Parent == null) return;

            Console.WriteLine($"{obj.Name} - rotateAroundParent");

            obj.SetRotation(Quaternion.CreateFromAxisAngle(Vector3.UnitY, deltaTime * 10), obj.Parent.CurrentPosition);
            
            if (obj.Parent.CurrentPosition != obj.Parent.PreviousPosition)
                obj.SetPosition(new Vector3(obj.CurrentPosition.X,  obj.Parent.CurrentPosition.Y, obj.CurrentPosition.Z));
        };

        pyramidSun.PermanentTransform += rotateAroundSelf;
        pyramidPlanet.PermanentTransform += rotateAroundSelf;
        pyramidMoon.PermanentTransform += rotateAroundSelf;

        pyramidPlanet.PermanentTransform += rotateAroundParent;
        pyramidMoon.PermanentTransform += rotateAroundParent;

        EventHandler<float> idk = (sender, deltaTime) =>
        {
            if (sender is not GameObject obj) return;

            Console.WriteLine($"{obj.Name} - idk");

            if (InputContext.Keyboards[0].IsKeyPressed(Key.Left))
            {
                obj.MoveBy(new Vector3((float)-deltaTime * 3, 0, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Right))
            {
                obj.MoveBy(new Vector3((float)deltaTime * 3, 0, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Up))
            {
                obj.MoveBy(new Vector3(0, (float)deltaTime * 3, 0));
            }
            if (InputContext.Keyboards[0].IsKeyPressed(Key.Down))
            {
                obj.MoveBy(new Vector3(0, (float)-deltaTime * 3, 0));
            }
        };

        pyramidSun.PermanentTransform += idk;



        var scene = new Scene(GL, Window, InputContext);
        scene.Objects.Add(pyramid);
        scene.Objects.Add(triangle);
        scene.Objects.Add(square);
        scene.Objects.Add(floor);
        scene.Objects.Add(pyramidSun);
        scene.Objects.Add(pyramidPlanet);
        scene.Objects.Add(pyramidMoon);

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
        Console.WriteLine($"**Update for frame {++counter}**");

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
    private static int counter = 0;
    private void OnRender(double deltaTime)
    {
        Console.WriteLine($"**Rendering frame {counter}**");

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

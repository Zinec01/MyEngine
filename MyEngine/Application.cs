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

public class Application
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


    public Application(int width, int height, string name)
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

        InputContext.Keyboards[0].KeyDown += OnKeyDown;

        InputContext.Mice[0].DoubleClickTime = 0;
        InputContext.Mice[0].Click += OnMouseClick;
        InputContext.Mice[0].MouseMove += OnMouseMove;
        InputContext.Mice[0].Scroll += OnMouseScroll;

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

        EventHandler<float> allAxesRotation = (sender, deltaTime) =>
        {
            if (sender is not GameObject obj) return;

            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)deltaTime / 2));
            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)deltaTime / 2));
            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)deltaTime / 2));
        };

        var pyramid = new GameObject(GL, "pyramid", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\obama.jpg", position: new Vector3(3f, 2f, 0));
        pyramid.PermanentTransform += allAxesRotation;

        var triangle = new GameObject(GL, "triangle", triangleVerts, triangleInds, @"..\..\..\..\MyEngine\Textures\obama.jpg", position: new Vector3(-3f, 2f, 0));
        triangle.PermanentTransform += allAxesRotation;

        var square = new GameObject(GL, "square", squareVerts, squareInds, @"..\..\..\..\MyEngine\Textures\obama.jpg", position: new Vector3(0f, 2f, 0f))
        {
            Parent = triangle
        };

        EventHandler<ParentObjectChangedArgs> copyParentRotationAndMovement = (sender, args) =>
        {
            if (sender is not GameObject obj) return;

            var parent = args.Parent;

            if (args.ChangeEvent.HasFlag(ObjectChangedFlag.ROTATION))
                obj.SetRotation(parent.CurrentRotation);

            if (args.ChangeEvent.HasFlag(ObjectChangedFlag.POSITION))
                obj.SetPosition(obj.CurrentPosition + (parent.CurrentPosition - parent.PreviousPosition));
        };

        square.ParentObjectChanged += copyParentRotationAndMovement;

        var floor = new GameObject(GL, "floor", squareVerts, squareInds, @"..\..\..\..\MyEngine\Textures\xd.png", rotation: Quaternion.CreateFromAxisAngle(Vector3.UnitX, -90f.DegToRad()), scale: 10f);


        var pyramidSun    = new GameObject(GL, "pyramidSun",    pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\teletubbies_sun.png", position: new Vector3(3f, 5f, 0f), scale: 0.50f);

        var pyramidPlanetRight = new GameObject(GL, "pyramidPlanetRight", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(5f, 5f, 0f), scale: 0.30f) { Parent = pyramidSun };
        var pyramidMoonRight   = new GameObject(GL, "pyramidMoonRight",   pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(6f, 5f, 0f), scale: 0.15f) { Parent = pyramidPlanetRight };
        var pyramidMoonRight2   = new GameObject(GL, "pyramidMoonRight2",   pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(6f, 5f, 0.5f), scale: 0.08f) { Parent = pyramidMoonRight };
        var pyramidMoonRight3   = new GameObject(GL, "pyramidMoonRight3",   pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(6f, 5f, 1f), scale: 0.06f) { Parent = pyramidMoonRight2 };

        var pyramidPlanetLeft = new GameObject(GL, "pyramidPlanetLeft", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(1f, 5f, 0f), scale: 0.30f) { Parent = pyramidSun };
        var pyramidMoonLeft = new GameObject(GL, "pyramidMoonLeft", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(0f, 5f, 0f), scale: 0.15f) { Parent = pyramidPlanetLeft };
        var pyramidMoonLeft2 = new GameObject(GL, "pyramidMoonLeft2", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(0f, 5f, -0.5f), scale: 0.08f) { Parent = pyramidMoonLeft };
        var pyramidMoonLeft3 = new GameObject(GL, "pyramidMoonLeft3", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(0f, 5f, -1f), scale: 0.06f) { Parent = pyramidMoonLeft2 };

        var pyramidPlanetFront = new GameObject(GL, "pyramidPlanetFront", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(3f, 5f, 2f), scale: 0.30f) { Parent = pyramidSun };
        var pyramidMoonFront = new GameObject(GL, "pyramidMoonFront", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(3f, 5f, 3f), scale: 0.15f) { Parent = pyramidPlanetFront };
        var pyramidMoonFront2 = new GameObject(GL, "pyramidMoonFront2", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(2.5f, 5f, 3f), scale: 0.08f) { Parent = pyramidMoonFront };
        var pyramidMoonFront3 = new GameObject(GL, "pyramidMoonFront3", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(2f, 5f, 3f), scale: 0.06f) { Parent = pyramidMoonFront2 };

        var pyramidPlanetBack = new GameObject(GL, "pyramidPlanetBack", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(3f, 5f, -2f), scale: 0.30f) { Parent = pyramidSun };
        var pyramidMoonBack = new GameObject(GL, "pyramidMoonBack", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(3f, 5f, -3f), scale: 0.15f) { Parent = pyramidPlanetBack };
        var pyramidMoonBack2 = new GameObject(GL, "pyramidMoonBack2", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(3.5f, 5f, -3f), scale: 0.08f) { Parent = pyramidMoonBack };
        var pyramidMoonBack3 = new GameObject(GL, "pyramidMoonBack3", pyramidVerts, pyramidInds, @"..\..\..\..\MyEngine\Textures\xd.png", position: new Vector3(4f, 5f, -3f), scale: 0.06f) { Parent = pyramidMoonBack2 };


        EventHandler<float> rotateAroundSelf = (sender, deltaTime) =>
        {
            if (sender is not GameObject obj) return;

            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, deltaTime * (1 - obj.CurrentScale) * 2));
        };

        EventHandler<float> rotateAroundParent = (sender, deltaTime) =>
        {
            if (sender is not GameObject obj || obj.Parent == null) return;

            if (obj.Parent.CurrentPosition != obj.Parent.PreviousPosition)
                obj.SetPosition(obj.CurrentPosition + (obj.Parent.CurrentPosition - obj.Parent.PreviousPosition));

            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, deltaTime), obj.Parent.CurrentPosition);
        };

        EventHandler<float> rotateAroundParent2 = (sender, deltaTime) =>
        {
            if (sender is not GameObject obj || obj.Parent == null)  return;

            if (obj.Parent.CurrentPosition != obj.Parent.PreviousPosition)
                obj.SetPosition(obj.CurrentPosition + (obj.Parent.CurrentPosition - obj.Parent.PreviousPosition));

            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, deltaTime * 2), obj.Parent.CurrentPosition);
        };

        EventHandler<float> rotateAroundParent3 = (sender, deltaTime) =>
        {
            if (sender is not GameObject obj || obj.Parent == null) return;

            if (obj.Parent.CurrentPosition != obj.Parent.PreviousPosition)
                obj.SetPosition(obj.CurrentPosition + (obj.Parent.CurrentPosition - obj.Parent.PreviousPosition));

            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, deltaTime * 5), obj.Parent.CurrentPosition);
        };

        EventHandler<float> rotateAroundParent4 = (sender, deltaTime) =>
        {
            if (sender is not GameObject obj || obj.Parent == null) return;

            if (obj.Parent.CurrentPosition != obj.Parent.PreviousPosition)
                obj.SetPosition(obj.CurrentPosition + (obj.Parent.CurrentPosition - obj.Parent.PreviousPosition));

            obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, deltaTime * 8), obj.Parent.CurrentPosition);
        };

        pyramidSun.PermanentTransform += rotateAroundSelf;

        pyramidPlanetRight.PermanentTransform += rotateAroundSelf;
        pyramidMoonRight.PermanentTransform += rotateAroundSelf;
        pyramidMoonRight2.PermanentTransform += rotateAroundSelf;
        pyramidMoonRight3.PermanentTransform += rotateAroundSelf;

        pyramidPlanetLeft.PermanentTransform += rotateAroundSelf;
        pyramidMoonLeft.PermanentTransform += rotateAroundSelf;
        pyramidMoonLeft2.PermanentTransform += rotateAroundSelf;
        pyramidMoonLeft3.PermanentTransform += rotateAroundSelf;

        pyramidPlanetFront.PermanentTransform += rotateAroundSelf;
        pyramidMoonFront.PermanentTransform += rotateAroundSelf;
        pyramidMoonFront2.PermanentTransform += rotateAroundSelf;
        pyramidMoonFront3.PermanentTransform += rotateAroundSelf;

        pyramidPlanetBack.PermanentTransform += rotateAroundSelf;
        pyramidMoonBack.PermanentTransform += rotateAroundSelf;
        pyramidMoonBack2.PermanentTransform += rotateAroundSelf;
        pyramidMoonBack3.PermanentTransform += rotateAroundSelf;


        pyramidPlanetRight.PermanentTransform += rotateAroundParent;
        pyramidMoonRight.PermanentTransform += rotateAroundParent2;
        pyramidMoonRight2.PermanentTransform += rotateAroundParent3;
        pyramidMoonRight3.PermanentTransform += rotateAroundParent4;

        pyramidPlanetLeft.PermanentTransform += rotateAroundParent;
        pyramidMoonLeft.PermanentTransform += rotateAroundParent2;
        pyramidMoonLeft2.PermanentTransform += rotateAroundParent3;
        pyramidMoonLeft3.PermanentTransform += rotateAroundParent4;

        pyramidPlanetFront.PermanentTransform += rotateAroundParent;
        pyramidMoonFront.PermanentTransform += rotateAroundParent2;
        pyramidMoonFront2.PermanentTransform += rotateAroundParent3;
        pyramidMoonFront3.PermanentTransform += rotateAroundParent4;

        pyramidPlanetBack.PermanentTransform += rotateAroundParent;
        pyramidMoonBack.PermanentTransform += rotateAroundParent2;
        pyramidMoonBack2.PermanentTransform += rotateAroundParent3;
        pyramidMoonBack3.PermanentTransform += rotateAroundParent4;

        EventHandler<float> idk = (sender, deltaTime) =>
        {
            if (sender is not GameObject obj) return;

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

        //pyramidSun.PermanentTransform += (sender, deltaTime) =>
        //{
        //    if (sender is not GameObject obj) return;

        //    obj.Rotate(Quaternion.CreateFromAxisAngle(Vector3.UnitY, deltaTime), new Vector3(0f, 5f, 0f));
        //};



        var scene = new Scene(GL, Window, InputContext);
        scene.Objects.Add(pyramid);
        scene.Objects.Add(triangle);
        scene.Objects.Add(square);
        scene.Objects.Add(floor);
        scene.Objects.Add(pyramidSun);
        scene.Objects.Add(pyramidPlanetRight);
        scene.Objects.Add(pyramidMoonRight);
        scene.Objects.Add(pyramidMoonRight2);
        scene.Objects.Add(pyramidMoonRight3);
        scene.Objects.Add(pyramidPlanetLeft);
        scene.Objects.Add(pyramidMoonLeft);
        scene.Objects.Add(pyramidMoonLeft2);
        scene.Objects.Add(pyramidMoonLeft3);
        scene.Objects.Add(pyramidPlanetFront);
        scene.Objects.Add(pyramidMoonFront);
        scene.Objects.Add(pyramidMoonFront2);
        scene.Objects.Add(pyramidMoonFront3);
        scene.Objects.Add(pyramidPlanetBack);
        scene.Objects.Add(pyramidMoonBack);
        scene.Objects.Add(pyramidMoonBack2);
        scene.Objects.Add(pyramidMoonBack3);

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
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit/* | ClearBufferMask.StencilBufferBit*/);

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

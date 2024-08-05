﻿using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Numerics;

namespace MyEngine
{
    internal class Program
    {
        public static GL GL { get; private set; }
        private static IWindow Window { get; set; }
        private static IInputContext InputContext { get; set; }
        private static ImGuiController ImGuiController { get; set; }

        public static int FPS { get; private set; }
        private static bool VSync;
        private static DateTime Started { get; set; }

        static void Main()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1280, 720);
            options.Title = "Hell yeah";
            options.VSync = true;

            Window = Silk.NET.Windowing.Window.Create(options);
            
            Window.Load += OnLoad;
            Window.Update += OnUpdate;
            Window.Render += OnRender;
            Window.FramebufferResize += OnFramebufferResize;
            Window.Closing += OnClose;

            Window.Run();

            Window.Dispose();
        }

        private static Scene Scene { get; set; }

        private static void OnLoad()
        {
            InputContext = Window.CreateInput();
            for (int i = 0; i < InputContext.Keyboards.Count; i++)
            {
                InputContext.Keyboards[i].KeyDown += KeyDown;
            }

            GL = GL.GetApi(Window);

            Console.WriteLine("VSync " + (Window.VSync ? "ON" : "OFF"));

            ImGuiController = new ImGuiController(GL, Window, InputContext);

            Started = DateTime.Now;
            VSync = Window.VSync;

            var triangleVerts = new[]
            {
                -0.5f, -0.5f, 0f,
                   0f,  0.5f, 0f,
                 0.5f, -0.5f, 0f
            };

            var triangle = new Model(triangleVerts);

            triangle.OnPermanentTransform += (sender, deltaTime) =>
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

            Scene = new Scene();
            Scene.TryAddModel(1, triangle);
        }

        private static void OnUpdate(double deltaTime)
        {
            if (DateTime.Now.Millisecond % 250 < 10)
            {
                FPS = (int)(1 / deltaTime);
                Console.WriteLine($"{FPS} FPS");

                if (Window.VSync != VSync)
                {
                    Window.VSync = VSync;
                    Thread.Sleep(20);
                }
            }

            Scene.UpdateObjects(deltaTime);
        }

        private static void OnRender(double deltaTime)
        {
            ImGuiController.Update((float)deltaTime);

            GL.ClearColor(0.07f, 0.01f, 0.02f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

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

        private static void KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            var model = Scene.Models.First().Value;
            switch (key)
            {
                case Key.Escape:
                    Window.Close();
                    break;

                //case Key.Right:
                //    model.Transform.Move(new Vector3(0.1f, 0, 0));
                //    break;

                //case Key.Left:
                //    model.Transform.Move(new Vector3(-0.1f, 0, 0));
                //    break;

                //case Key.Up:
                //    model.Transform.Move(new Vector3(0, 0.1f, 0));
                //    break;

                //case Key.Down:
                //    model.Transform.Move(new Vector3(0, -0.1f, 0));
                //    break;
            }
        }

        private static void OnClose()
        {
            Scene.Dispose();
            GL.Dispose();

            Console.WriteLine("GG");
        }
    }
}

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace MyEngine
{
    internal class Program
    {
        public static GL GL { get; private set; }
        private static IWindow Window { get; set; }

        private static DateTime Started { get; set; }

        static void Main(string[] args)
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
            var input = Window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }

            GL = GL.GetApi(Window);

            Started = DateTime.Now;

            var triangle = new[]
            {
                -0.5f, -0.5f, 0f,
                   0f,  0.5f, 0f,
                 0.5f, -0.5f, 0f
            };

            var model = new Model(triangle);

            Scene = new Scene();
            Scene.TryAddModel(1, model);
        }

        private static void OnUpdate(double deltaTime)
        {
            if (DateTime.Now.Millisecond % 250 < 10)
            {
                Console.WriteLine($"{1 / deltaTime:0} FPS");
            }
        }

        private static void OnRender(double deltaTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Scene.Draw();
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            GL.Viewport(newSize);
            Console.WriteLine($"New resolution: {newSize.X}x{newSize.Y}");
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            if (key == Key.Escape)
            {
                Window.Close();
            }
        }

        private static void OnClose()
        {
            Console.WriteLine("GG");
        }
    }
}

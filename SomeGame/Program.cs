//using MyEngine;
using ECSEngineTest;

namespace SomeGame;

internal class Program
{
    static void Main(string[] args)
    {
        using var app = new Application(new WindowSettings
        {
            Width = 1280,
            Height = 720,
            Title = "Hell yeah",
            VSync = true
        });
        app.MainWindow.OnLoad += () =>
        {
            var scene = app.CreateScene("Test");
            scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\chest_plane_light_camera.fbx");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\chest_plane_light_camera.glb");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\spider_pink_cube_light.glb");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\spider_light.glb");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\spider_light.gltf");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\pink_cube.glb");

            var program = scene.ShaderManager.GetShaderProgram("Basic",
                                                               @"..\..\..\..\MyEngine\Shaders\basic_light.vert",
                                                               @"..\..\..\..\MyEngine\Shaders\basic_light.frag");
        };
        app.Run();
        //new Application(1280, 720, "lmao").Run();
    }
}

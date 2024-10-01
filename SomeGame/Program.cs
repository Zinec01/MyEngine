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
        app.MainWindow.OnLoad += (sender, args) =>
        {
            var scene = app.CreateScene("Test");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\whole_scene.glb", SceneLoadFlags.Meshes);//.Everything);
            //scene.LoadObjects(@"C:\Users\Zinec\Desktop\spider_scene.glb");
            //scene.LoadObjects(@"C:\Users\Zinec\Desktop\test2.obj");
            //scene.LoadObjects(@"C:\Users\Zinec\Desktop\test2.glb");
            //scene.LoadObjects(@"C:\Users\Zinec\Desktop\spider_scene.gltf");
            //scene.LoadObjects(@"C:\Users\Zinec\Desktop\test.glb");

            var program = scene.ShaderManager.GetShaderProgram("Basic",
                                                               @"..\..\..\..\MyEngine\Shaders\basic_light.vert",
                                                               @"..\..\..\..\MyEngine\Shaders\basic_light.frag");
        };
        app.Run();
        //new Application(1280, 720, "lmao").Run();
    }
}

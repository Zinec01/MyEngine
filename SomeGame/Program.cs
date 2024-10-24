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
            scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Models\concrete_trash_bin.glb");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\chest_plane_light_camera.fbx");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\chest_plane_light_camera.glb");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\spider_pink_cube_light.glb");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\spider_light.glb");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\spider_light.gltf");
            //scene.Loader.LoadScene(@"C:\Users\Zinec\Desktop\pink_cube.glb");

            scene.EntityFactory.CreateCamera("Main Camera")
                               .WithPosition(new System.Numerics.Vector3(0.0f, 2.0f, -5.0f))
                               .Build();
        };
        app.Run();
        //new Application(1280, 720, "lmao").Run();
    }
}

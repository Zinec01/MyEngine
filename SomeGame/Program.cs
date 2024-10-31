//using MyEngine;
using ECSEngineTest;
using System.Numerics;

namespace SomeGame;

public class Program
{
    public static void Main(string[] args)
    {
        using var app = new Application(new WindowSettings
        {
            Width = 1280,
            Height = 720,
            Title = "Hell yeah",
            VSync = true
        });
        app.Init += InitApp;
        app.Run();
        //new Application(1280, 720, "lmao").Run();
    }

    private static void InitApp(Application app)
    {
        var scene = app.CreateScene("Test");

        scene.EntityFactory.CreateCamera("Main Camera")
                           .SetPosition(new Vector3(0.0f, 0.0f, 20.0f))
                           .Build();

        //scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Assets\Models\concrete_trash_bin_fixed_transforms.glb");
        //scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Assets\Models\concrete_trash_bin_fixed_transforms.gltf");
        //scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Assets\Models\concrete_trash_bin.gltf");
        //scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Assets\Models\concrete_trash_bin.glb");
        //scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Assets\Models\chest_plane_light_camera.fbx");
        //scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Assets\Models\chest_plane_light_camera.glb");
        //scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Assets\Models\spider_pink_cube_light.glb");
        //scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Assets\Models\spider_light.glb");
        //scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Assets\Models\spider_light.gltf");
        //scene.Loader.LoadScene(@"..\..\..\..\ECSEngineTest\Assets\Models\pink_cube.glb");
    }
}

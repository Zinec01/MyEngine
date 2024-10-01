﻿namespace ECSEngineTest;

public class Application : IDisposable
{
    public Window MainWindow { get; } //TODO: make private and introduce window events instead

    private readonly List<Scene> _scenes = [];
    public IReadOnlyList<Scene> Scenes => _scenes;
    public int? ActiveSceneId { get; set; }
    public Scene? ActiveScene => ActiveSceneId.HasValue ? _scenes.FirstOrDefault(x => x.Id == ActiveSceneId) : null;

    public Application(WindowSettings windowSettings)
    {
        MainWindow = new(windowSettings);
    }

    public Scene CreateScene(string name)
    {
        var scene = new Scene(name);
        _scenes.Add(scene);
        return scene;
    }

    public void Run()
    {
        MainWindow?.Run();
    }

    public void Dispose()
    {
        foreach (var scene in _scenes)
        {
            scene.Dispose();
        }
        Window.GL.Dispose();
    }
}

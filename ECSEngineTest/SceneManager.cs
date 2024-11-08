namespace ECSEngineTest;

public static class SceneManager
{
    private static readonly List<Scene> _scenes = [];
    public static IReadOnlyList<Scene> Scenes => _scenes;

    public static Scene? ActiveScene { get; private set; } = null;

    public static Scene CreateScene(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = "Scene";
        }

        int i = 0;
        var nameOccurances = _scenes.Count(x =>
        {
            var split = x.Name.Split(' ');
            var last = split[^1];

            return last[0] == '('
                && last[^1] == ')'
                && int.TryParse(last[1..^2], out i)
                && x.Name[..^(last.Length + 1)] == name;
        });

        if (nameOccurances > 0)
            name += $" ({i + 1})";

        var scene = new Scene(name);

        if (_scenes.Count == 0)
            ActiveScene = scene;
        else
            scene.Enabled = false;

        _scenes.Add(scene);

        return scene;
    }

    public static void DeleteScene(Scene scene)
    {
        if (scene is null) return;

        if (scene.Enabled)
        {
            if (_scenes.Count > 1)
            {
                _scenes[0].Enabled = true;
                ActiveScene = _scenes[0];
            }
            else
            {
                ActiveScene = null;
            }
        }

        _scenes.Remove(scene);
        LayerManager.RemoveLayer(scene);

        scene.Dispose();
    }

    public static void SetActive(Scene scene)
    {
        if (scene is null || scene.Enabled) return;

        var currentScene = _scenes.FirstOrDefault(x => x.Id == scene?.Id);
        if (currentScene != null)
            currentScene.Enabled = false;

        scene.Enabled = true;

        LayerManager.AddLayer(scene);
    }

    public static void Dispose()
    {
        foreach (var scene in _scenes)
        {
            scene.Dispose();
        }
    }
}

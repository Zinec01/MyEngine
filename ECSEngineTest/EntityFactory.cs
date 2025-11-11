using ECSEngineTest.Components;
using ECSEngineTest.Builders;
using Friflo.Engine.ECS;

namespace ECSEngineTest;

public class EntityFactory(EntityStore store)
{
    private int _count = 0;

    public Entity CreateEntity(string? name = null)
    {
        return store.CreateEntity(new EntityName(!string.IsNullOrEmpty(name) ? name : $"Entity-{_count++}"));
    }

    public Entity CreateTexturedObject(string? name = null)
    {
        var entity = CreateEntity(name);

        entity.AddComponent<TransformComponent>();
        entity.AddComponent<ShaderProgramComponent>();
        entity.AddComponent<TextureComponent>();

        return entity;
    }

    public Entity CreateColorObject(string? name = null)
    {
        var entity = CreateEntity(name);

        entity.AddComponent<TransformComponent>();
        entity.AddComponent<ShaderProgramComponent>();
        entity.AddComponent<ColorComponent>();

        return entity;
    }

    public Entity CreateLight(string? name = null)
    {
        var entity = CreateEntity(name);

        entity.AddComponent<LightComponent>();
        entity.AddComponent<TransformComponent>();
        entity.AddComponent<ColorComponent>();

        return entity;
    }

    public CameraBuilder CreateCamera(string name) => new(store, name);
}

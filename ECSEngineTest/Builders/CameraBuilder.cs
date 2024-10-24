using ECSEngineTest.Components;
using Friflo.Engine.ECS;
using System.Numerics;

namespace ECSEngineTest.Builders;

public class CameraBuilder(EntityStore store, string name)
{
    private string name = name;
    private Quaternion rotation = Quaternion.Identity;
    private Vector3 position = Vector3.Zero;

    public CameraBuilder WithRotation(Quaternion rotation)
    {
        this.rotation = rotation;

        return this;
    }

    public CameraBuilder WithPosition(Vector3 position)
    {
        this.position = position;

        return this;
    }

    public void Build()
    {
        store.CreateEntity(new EntityName(name),
            new CameraComponent
            {
                Active = !store.Entities.Any(x => x.HasComponent<CameraComponent>()
                                                  && x.GetComponent<CameraComponent>().Active)
            }, new TransformComponent
            {
                Position = new Interpolatable<Vector3>(position),
                Rotation = new Interpolatable<Quaternion>(rotation)
            });

        name = string.Empty;
        rotation = Quaternion.Identity;
        position = Vector3.Zero;
    }
}

using ECSEngineTest.Tags;
using Friflo.Engine.ECS;

namespace ECSEngineTest;

public class Renderer
{
    public static void RenderScene(EntityStore store)
    {
        store.Query<EntityName>()
             .AllTags(Friflo.Engine.ECS.Tags.Get<MeshObjectTag>())
             .ForEachEntity((ref EntityName name, Entity entity) =>
        {

        });
    }
}

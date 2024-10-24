using ECSEngineTest.Components;
using ECSEngineTest.Tags;
using Friflo.Engine.ECS;
using Silk.NET.OpenGL;

namespace ECSEngineTest;

public class Renderer
{
    public static unsafe void RenderScene(EntityStore store, double deltaTime)
    {
        store.Query<TransformComponent, ShaderProgramComponent>()
             .AllTags(Friflo.Engine.ECS.Tags.Get<MeshObjectTag>())
             .ForEachEntity((ref TransformComponent transform,
                             ref ShaderProgramComponent shaderProgram,
                             Entity entity) =>
        {
            ShaderManager.UseShaderProgram(shaderProgram.Id);

            foreach (var child in entity.ChildEntities)
            {
                var mesh = child.GetComponent<MeshComponent>();
                MeshManager.BindVAO(mesh.VAO);

                ShaderManager.SetUniform(shaderProgram.Id, "uModelMat", transform.WorldTransform);

                TextureComponent? texture = null;
                if (child.HasComponent<TextureComponent>())
                {
                    texture = child.GetComponent<TextureComponent>();
                    TextureManager.ActivateTexture(texture.Value.Id);

                    ShaderManager.SetUniform(shaderProgram.Id, "uTex", 0);
                }

                Window.GL.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);

                if (texture.HasValue)
                {
                    TextureManager.DeactivateCurrentTexture();
                }
            }
        });
    }
}

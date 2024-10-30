using ECSEngineTest.Components;
using ECSEngineTest.Tags;
using Friflo.Engine.ECS;
using Silk.NET.OpenGL;
using System.Numerics;

namespace ECSEngineTest;

public class Renderer
{
    public static unsafe void RenderScene(EntityStore store, double deltaTime)
    {
        store.Query<CameraComponent, TransformComponent>()
             .ForEachEntity((ref CameraComponent camera, ref TransformComponent transform, Entity entity) =>
        {
            if (!camera.Active) return;

            //CameraManager.SetUBOViewMat(ref camera);
        });

        var i = 0;
        Entity? parent = null;
        store.Query<TransformComponent>()
             .AllTags(Friflo.Engine.ECS.Tags.Get<MeshObjectTag>())
             .ForEachEntity((ref TransformComponent transform, Entity entity) =>
        {
            if (parent == null)
            {
                if (!entity.Parent.IsNull)
                {
                    parent = entity.Parent;
                }
                else
                {
                    parent = entity;
                }
            }
            else
            {
                if (!entity.Parent.IsNull && parent != entity.Parent)
                {
                    parent = entity.Parent;
                    i++;
                }
                else if(entity.Parent.IsNull && parent != entity)
                {
                    parent = entity;
                    i++;
                }
            }

            var modulo = i % 4;
            var increment = ((i / 4) + 1) * 2;
            var x = modulo > 0 && modulo < 3 ? increment : -increment;
            var y = modulo < 2 ? increment : -increment;

            foreach (var child in entity.ChildEntities)
            {
                if (!child.HasComponent<MeshComponent>() || !child.HasComponent<ShaderProgramComponent>())
                    continue;

                var shaderProgram = child.GetComponent<ShaderProgramComponent>();

                ShaderManager.UseShaderProgram(shaderProgram.Id);

                var mesh = child.GetComponent<MeshComponent>();
                MeshManager.BindVAO(mesh.VAO);

                var tmpEntity = entity;
                var tmpTransform = transform.WorldTransform;
                while (!tmpEntity.Parent.IsNull)
                {
                    var parentTransform = tmpEntity.Parent.GetComponent<TransformComponent>().WorldTransform;
                    if (!parentTransform.IsIdentity)
                        tmpTransform *= parentTransform;

                    tmpEntity = tmpEntity.Parent;
                }

                tmpTransform *= Matrix4x4.CreateTranslation(new Vector3(x, y, 0));

                ShaderManager.SetUniformVariable(shaderProgram.Id, ShaderUniforms.ModelMatrix, tmpTransform);

                if (child.HasComponent<TextureComponent>())
                {
                    var texture = child.GetComponent<TextureComponent>();
                    TextureManager.ActivateTexture(texture.Id);

                    ShaderManager.SetUniformVariable(shaderProgram.Id, ShaderUniforms.TextureSampler, 0);
                }
                else if (child.HasComponent<ColorComponent>())
                {
                    var color = child.GetComponent<ColorComponent>();
                    ShaderManager.SetUniformVariable(shaderProgram.Id, ShaderUniforms.Color, color.Color);
                }

                Window.GL.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
            }
        });
    }
}

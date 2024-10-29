using ECSEngineTest.Components;
using ECSEngineTest.Tags;
using Friflo.Engine.ECS;
using Silk.NET.OpenGL;

namespace ECSEngineTest;

public class Renderer
{
    public static unsafe void RenderScene(EntityStore store, double deltaTime)
    {
        store.Query<CameraComponent>()
             .ForEachEntity((ref CameraComponent camera, Entity entity) =>
        {
            if (!camera.Active) return;

            CameraManager.SetUBOViewMat(ref camera);
        });

        store.Query<TransformComponent>()
             .AllTags(Friflo.Engine.ECS.Tags.Get<MeshObjectTag>())
             .ForEachEntity((ref TransformComponent transform, Entity entity) =>
        {
            foreach (var child in entity.ChildEntities)
            {
                var shaderProgram = child.GetComponent<ShaderProgramComponent>();

                ShaderManager.UseShaderProgram(shaderProgram.Id);

                //ShaderManager.SetUniform(shaderProgram.Id, ShaderUniforms.ProjectionMatrix, projectMat);
                //ShaderManager.SetUniform(shaderProgram.Id, ShaderUniforms.ViewMatrix, viewMat);

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

                ShaderManager.SetUniform(shaderProgram.Id, ShaderUniforms.ModelMatrix, tmpTransform);

                if (child.HasComponent<TextureComponent>())
                {
                    var texture = child.GetComponent<TextureComponent>();
                    TextureManager.ActivateTexture(texture.Id);

                    ShaderManager.SetUniform(shaderProgram.Id, ShaderUniforms.TextureSampler, 0);
                }
                else if (child.HasComponent<ColorComponent>())
                {
                    var color = child.GetComponent<ColorComponent>();
                    ShaderManager.SetUniform(shaderProgram.Id, ShaderUniforms.Color, color.Color);
                }

                Window.GL.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
            }
        });

        //var triangleVerts = new[]
        //{
        //    new Vector3(-1f, -1f, 0f),
        //    new Vector3( 1f, -1f, 0f),
        //    new Vector3( 0f,  1f, 0f)
        //};

        //var triangleNormals = new[]
        //{
        //    new Vector3(0f, 0f, 1f),
        //    new Vector3(0f, 0f, 1f),
        //    new Vector3(0f, 0f, 1f)
        //};

        //var triangleUvs = new[]
        //{
        //    new Vector2(  0f, 1f),
        //    new Vector2(  1f, 1f),
        //    new Vector2(0.5f, 0f)
        //};

        //var triangleInds = new uint[] { 0, 1, 2 };

        //var meshComponent = MeshManager.CreateMeshComponent(new Span<Vector3>(triangleVerts),
        //                                                    new Span<Vector3>(triangleNormals),
        //                                                    new Span<Vector2>(triangleUvs),
        //                                                    triangleInds);

        //_shaderManager ??= new(store);
        //shaderProgram ??= _shaderManager.GetShaderProgram("Test",
        //                                                  @"..\..\..\..\MyEngine\Shaders\basic_test.vert",
        //                                                  @"..\..\..\..\MyEngine\Shaders\basic_test.frag");

        //ShaderManager.UseShaderProgram(shaderProgram.Value.Id);

        //MeshManager.BindVAO(meshComponent.VAO);

        //ShaderManager.SetUniform(shaderProgram.Value.Id, ShaderUniforms.ModelMatrix, modelMat);

        //TextureManager.ActivateTexture(texture.Id);
        //ShaderManager.SetUniform(shaderProgram.Value.Id, ShaderUniforms.TextureSampler, 0);

        //Window.GL.DrawElements(PrimitiveType.Triangles, (uint)meshComponent.Indices.Length, DrawElementsType.UnsignedInt, null);

        //TextureManager.DeactivateCurrentTexture();
    }

    //private static ShaderManager? _shaderManager = null;
    //private static ShaderProgramComponent? shaderProgram = null;

    //private static readonly Matrix4x4 modelMat = Matrix4x4.CreateScale(new Vector3(1, 1, 1)) * Matrix4x4.CreateFromQuaternion(Quaternion.Identity) * Matrix4x4.CreateTranslation(new Vector3(0, 0, 0.0f));
    //private static readonly TextureComponent texture = TextureManager.GetTexture(@"C:\Users\Zinec\source\repos\MyEngine\MyEngine\Textures\obama.jpg");
}

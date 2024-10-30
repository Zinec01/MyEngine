using ECSEngineTest.Components;
using ECSEngineTest.Tags;
using Friflo.Engine.ECS;
using Silk.NET.OpenGL;

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

            //Console.WriteLine();
            //Console.WriteLine("Camera");
            //Console.WriteLine("FOV: " + camera.FieldOfView);
            //Console.WriteLine("Aspect Ratio: " + camera.AspectRatio);
            //Console.WriteLine("Near Plane: " + camera.NearPlane);
            //Console.WriteLine("Far Plane: " + camera.FarPlane);
            //Console.WriteLine("Position: " + transform.Position.Current);
            //Console.WriteLine("Rotation: " + transform.Rotation.Current);
            //Console.WriteLine("Scale: " + transform.Scale.Current);
            //Console.WriteLine("Projection Matrix: " + camera.ProjectMat);
            //Console.WriteLine("View Matrix: " + camera.ViewMat);
        });

        store.Query<TransformComponent>()
             .AllTags(Friflo.Engine.ECS.Tags.Get<MeshObjectTag>())
             .ForEachEntity((ref TransformComponent transform, Entity entity) =>
        {
            //Console.WriteLine();
            //Console.WriteLine($"Model - {entity.Name}");
            //Console.WriteLine("Position: " + transform.Position.Current);
            //Console.WriteLine("Rotation: " + transform.Rotation.Current);
            //Console.WriteLine("Scale: " + transform.Scale.Current);
            //Console.WriteLine("Transform: " + transform.WorldTransform);

            foreach (var child in entity.ChildEntities)
            {
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

                //Console.WriteLine();
                //Console.WriteLine($"Mesh - {child.Name}");
                //Console.WriteLine("VAO: " + mesh.VAO);
                //Console.WriteLine("VBO: " + mesh.VBO);
                //Console.WriteLine("EBO: " + mesh.EBO);
                //Console.WriteLine("Position: " + transform.Position.Current);
                //Console.WriteLine("Rotation: " + transform.Rotation.Current);
                //Console.WriteLine("Scale: " + transform.Scale.Current);
                //Console.WriteLine("Transform: " + tmpTransform);

                //Console.WriteLine();
                //Console.WriteLine("Vertices:");
                //foreach (var vertex in mesh.VertexTextureData?.Select(x => x.Vertex) ?? mesh.VertexData!.Select(x => x.Vertex))
                //{
                //    Console.WriteLine($"{vertex.X}, {vertex.Y}, {vertex.Z}");
                //}
                //Console.WriteLine();
                //Console.WriteLine("Normals:");
                //foreach (var normal in mesh.VertexTextureData?.Select(x => x.Normal) ?? mesh.VertexData!.Select(x => x.Normal))
                //{
                //    Console.WriteLine($"{normal.X}, {normal.Y}, {normal.Z}");
                //}
                //Console.WriteLine();
                //Console.WriteLine("Indices:");
                //var x = 0;
                //while (x < mesh.Indices.Length)
                //{
                //    Console.WriteLine($"{mesh.Indices[x++]}, {mesh.Indices[x++]}, {mesh.Indices[x++]}");
                //}
            }
        });
    }
}

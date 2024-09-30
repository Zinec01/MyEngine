using ECSEngineTest.Components;
using ECSEngineTest.Tags;
using Friflo.Engine.ECS;
using Silk.NET.Assimp;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ECSEngineTest;

public class SceneLoader(EntityStore entityStore) : IDisposable
{
    private readonly string[] SUPPORTED_FORMATS = [".obj", ".glb", ".gltf"];

    private readonly Assimp _assimp = Assimp.GetApi();
    private readonly EntityStore _entityStore = entityStore;

    private string _currentDirectory = string.Empty;

    public unsafe void LoadScene(string filePath, SceneLoadFlags flags = SceneLoadFlags.Everything)
    {
        var extension = Path.GetExtension(filePath);
        if (!SUPPORTED_FORMATS.Contains(extension))
            throw new NotImplementedException("Format not yet supported");

        var scene = LoadSceneFromFile(filePath);

        _currentDirectory = Path.GetDirectoryName(filePath)!;

        uint processed = 0;

        if (flags.HasFlag(SceneLoadFlags.Meshes))
            processed += ProcessMeshes(null, scene->MRootNode, scene, flags.HasFlag(SceneLoadFlags.WorldTransforms));

        if (flags.HasFlag(SceneLoadFlags.Lights))
            processed += ProcessLights(scene);

        if (processed > 0)
            FileChangeWatcher.SubscribeForFileChanges(filePath, (_, _) => SceneFileChanged(filePath));

        _currentDirectory = string.Empty;
    }

    private unsafe Silk.NET.Assimp.Scene* LoadSceneFromFile(string filePath)
    {
        var scene = _assimp.ImportFile(filePath, (uint)(PostProcessSteps.Triangulate
                                                        | PostProcessSteps.GenerateSmoothNormals
                                                        | PostProcessSteps.FlipUVs
                                                        | PostProcessSteps.JoinIdenticalVertices));

        if (scene == null || scene->MFlags == Assimp.SceneFlagsIncomplete || scene->MRootNode == null)
        {
            var error = _assimp.GetErrorStringS();
            throw new Exception(error);
        }

        return scene;
    }

    private unsafe uint ProcessMeshes(Entity? parentEntity, Node* node, Silk.NET.Assimp.Scene* scene, bool loadTransforms = false)
    {
        Entity? entity = null;
        uint processed = 0;
        if (node->MNumMeshes > 0 && node->MParent is not null)
        {
            entity = _entityStore.CreateEntity(new EntityName(node->MName.ToString()));
            entity.Value.AddTag<MeshObjectTag>();

            var transformComponent = new TransformComponent();
            if (loadTransforms)
                transformComponent.WorldTransform = node->MTransformation;

            entity.Value.AddComponent(transformComponent);

            if (node->MParent->MNumMeshes > 0)
                parentEntity?.AddChild(entity.Value);

            for (int i = 0; i < node->MNumMeshes; i++)
            {
                var meshIdx = node->MMeshes[i];
                ProcessMesh(entity.Value, scene->MMeshes[meshIdx], scene);
            }

            processed += node->MNumMeshes + 1;
        }
        
        for (int i = 0; i < node->MNumChildren; i++)
        {
            processed += ProcessMeshes(entity, node->MChildren[i], scene, loadTransforms);
        }

        return processed;
    }

    private unsafe void ProcessMesh(Entity parentEntity, Mesh* mesh, Silk.NET.Assimp.Scene* scene)
    {
        var entity = _entityStore.CreateEntity();
        parentEntity.AddChild(entity);

        var material = scene->MMaterials[mesh->MMaterialIndex];
        var texCount = _assimp.GetMaterialTextureCount(material, TextureType.Diffuse);

        if (texCount > 0)
        {
            ProcessMeshVertices(entity, mesh, true);
            ProcessMeshTextures(entity, material, scene, texCount);
        }
        else
        {
            ProcessMeshVertices(entity, mesh, false);
            ProcessMeshColor(entity, material);
        }
    }

    private unsafe void ProcessMeshVertices(Entity entity, Mesh* mesh, bool isTexture)
    {
        var indices = new List<uint>();

        for (int i = 0; i < mesh->MNumFaces; i++)
        {
            var face = mesh->MFaces[i];
            for (int j = 0; j < face.MNumIndices; j++)
            {
                indices.Add(face.MIndices[j]);
            }
        }
        
        entity.AddComponent(MeshManager.CreateMeshComponent(new Span<Vector3>(mesh->MVertices, (int)mesh->MNumVertices),
                                                            new Span<Vector3>(mesh->MNormals, (int)mesh->MNumVertices),
                                                            isTexture ? new Span<Vector2>(mesh->MTextureCoords.Element0, (int)mesh->MNumVertices) : null,
                                                            [.. indices]));
    }

    private unsafe void ProcessMeshTextures(Entity entity, Material* material, Silk.NET.Assimp.Scene* scene, uint texCount)
    {
        for (uint i = 0; i < texCount; i++)
        {
            var path = new AssimpString();
            if (_assimp.GetMaterialTexture(material, TextureType.Diffuse, i, &path, null, null, null, null, null, null) == Return.Success
                && !string.IsNullOrEmpty(path))
            {
                var textureName = path.ToString();

                string texturePath;
                TextureComponent textureComponent;
                if (textureName.StartsWith('*') && int.TryParse(textureName.AsSpan(1), out var texIdx))
                {
                    var texture = scene->MTextures[texIdx];
                    var extension = Marshal.PtrToStringAnsi((nint)texture->AchFormatHint);
                    texturePath = Path.Combine(_currentDirectory, texture->MFilename) + "." + extension;

                    if (TextureManager.IsLoaded(texturePath))
                    {
                        textureComponent = TextureManager.GetTexture(texturePath);
                    }
                    else
                    {
                        var textureData = new Span<byte>((byte*)texture->PcData, (int)texture->MWidth * sizeof(Texel));
                        textureComponent = TextureManager.LoadTexture(texturePath, textureData.ToArray());
                    }
                }
                else
                {
                    texturePath = Path.Combine(_currentDirectory, textureName);
                    textureComponent = TextureManager.GetTexture(texturePath);
                }

                entity.AddComponent(textureComponent);
            }
        }
    }

    private unsafe void ProcessMeshColor(Entity entity, Material* material)
    {
        var color = new Vector4();
        if (_assimp.GetMaterialColor(material, Assimp.MaterialColorDiffuseBase, 0, 0, &color) == Return.Success)
        {
            entity.AddComponent(new ColorComponent(color));
        }
    }

    private unsafe uint ProcessLights(Silk.NET.Assimp.Scene* scene)
    {
        for (int i = 0; i < scene->MNumLights; i++)
        {
            var light = scene->MLights[i];

            var entity = _entityStore.CreateEntity(new EntityName(light->MName));
            entity.AddTag<LightTag>();
            entity.AddComponent(new LightComponent
            {
                Type = (LightType)(int)light->MType,
                Attenuation = light->MAttenuationQuadratic,
                InnerConeAngle = light->MAngleInnerCone,
                OuterConeAngle = light->MAngleOuterCone,
            });
            entity.AddComponent(new ColorComponent(new Vector4(light->MColorDiffuse, 1f)));
            entity.AddComponent(new TransformComponent { Position = new Interpolatable<Vector3>(light->MPosition) });
        }

        return scene->MNumLights;
    }

    private unsafe void MeshFileChanged(string filePath)
    {
    }

    private unsafe void SceneFileChanged(string filePath)
    {
    }

    public void Dispose()
    {
        _assimp.Dispose();
    }
}

[Flags]
public enum SceneLoadFlags
{
    None = 0,
    Meshes = 1,
    Lights = 2,
    WorldTransforms = 4,

    Everything = (1 << (4/*num of other enum values*/ - 1)) - 1
}

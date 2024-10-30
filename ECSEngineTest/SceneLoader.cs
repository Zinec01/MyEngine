using ECSEngineTest.Components;
using ECSEngineTest.Helpers;
using ECSEngineTest.Tags;
using Friflo.Engine.ECS;
using Silk.NET.Assimp;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ECSEngineTest;

public class SceneLoader : IDisposable
{
    private readonly Assimp _assimp = Assimp.GetApi();
    private readonly EntityStore _entityStore;
    private readonly ShaderManager _shaderManager;
    private readonly EntityFactory _entityFactory;

    private string _currentDirectory = string.Empty;

    public SceneLoader(EntityStore entityStore, ShaderManager shaderManager, EntityFactory entityFactory)
    {
        _entityStore = entityStore;
        _shaderManager = shaderManager;
        _entityFactory = entityFactory;
    }

    public unsafe void LoadScene(string filePath, SceneLoadFlags flags = SceneLoadFlags.Everything)
    {
        if (!FileHelper.ValidateFilePath(ref filePath))
            throw new FileNotFoundException(null, filePath);

        var scene = LoadSceneFromFile(filePath);

        _currentDirectory = Path.GetDirectoryName(filePath)!;

        uint processed = 0;
        Entity? rootEntity = null;
        if (flags.HasFlag(SceneLoadFlags.Meshes))
            processed += ProcessMeshes(null, scene->MRootNode, scene, out rootEntity);

        if (flags.HasFlag(SceneLoadFlags.Lights))
            processed += ProcessLights(rootEntity, scene);

        if (flags.HasFlag(SceneLoadFlags.Cameras))
            processed += ProcessCameras(rootEntity, scene);

        if (processed > 0)
            FileChangeWatcher.SubscribeForFileChanges(filePath, (_, _) => SceneFileChanged(filePath));

        _currentDirectory = string.Empty;
    }

    private unsafe Silk.NET.Assimp.Scene* LoadSceneFromFile(string filePath)
    {
        var scene = _assimp.ImportFile(filePath, (uint)(PostProcessSteps.Triangulate
                                                        | PostProcessSteps.GenerateSmoothNormals
                                                        | PostProcessSteps.FlipUVs
                                                        | PostProcessSteps.JoinIdenticalVertices
                                                        | PostProcessSteps.ImproveCacheLocality));

        if (scene == null || scene->MFlags == Assimp.SceneFlagsIncomplete || scene->MRootNode == null)
        {
            var error = _assimp.GetErrorStringS();
            throw new Exception(error);
        }

        return scene;
    }

    private unsafe uint ProcessMeshes(Entity? parentEntity, Node* node, Silk.NET.Assimp.Scene* scene, out Entity? rootEntity)
    {
        Entity? entity = null;
        rootEntity = null;

        uint processed = 0;
        if (node == scene->MRootNode)
        {
            entity = _entityStore.CreateEntity(new EntityName(node->MName));
            entity.Value.AddTag<NodeTag>();
            entity.Value.AddComponent(new TransformComponent(node->MTransformation));

            rootEntity = entity.Value;

            processed += 1;
        }
        if (node->MNumMeshes > 0)
        {
            if (entity == null)
            {
                entity = _entityStore.CreateEntity(new EntityName(node->MName));
                entity.Value.AddComponent(new TransformComponent(node->MTransformation));
            }
            entity.Value.AddTag<MeshObjectTag>();

            parentEntity?.AddChild(entity.Value);

            //var nodeTmp = node;
            //var entityTmp = entity.Value;
            //while (nodeTmp->MParent != null && nodeTmp != scene->MRootNode)
            //{
            //    var parentEntityTmp = _entityStore.CreateEntity(new EntityName(node->MParent->MName));
            //    parentEntityTmp.AddTag<NodeTag>();
            //    parentEntityTmp.AddComponent(new TransformComponent(node->MParent->MTransformation));
            //    parentEntityTmp.AddChild(entityTmp);

            //    entityTmp = parentEntityTmp;
            //    nodeTmp = node->MParent;
            //}

            for (int i = 0; i < node->MNumMeshes; i++)
            {
                var meshIdx = node->MMeshes[i];
                ProcessMesh(entity.Value, scene->MMeshes[meshIdx], scene);
            }

            processed += node->MNumMeshes + 1;
        }
        
        for (int i = 0; i < node->MNumChildren; i++)
        {
            processed += ProcessMeshes(entity, node->MChildren[i], scene, out _);
        }

        return processed;
    }

    private unsafe void ProcessMesh(Entity parentEntity, Mesh* mesh, Silk.NET.Assimp.Scene* scene)
    {
        var entity = _entityStore.CreateEntity(new EntityName(mesh->MName));
        parentEntity.AddChild(entity);

        var material = scene->MMaterials[mesh->MMaterialIndex];

        AssimpString materialName;
        _assimp.GetMaterialString(material, Assimp.MaterialNameBase, 0, 0, &materialName);

        // TODO: Handle material properties
        entity.AddComponent(new MaterialComponent { Name = materialName });

        //TODO: Handle different texture types
        var texCount = _assimp.GetMaterialTextureCount(material, TextureType.Diffuse);
        //texCount += _assimp.GetMaterialTextureCount(material, TextureType.Ambient);
        //texCount += _assimp.GetMaterialTextureCount(material, TextureType.BaseColor);
        //texCount += _assimp.GetMaterialTextureCount(material, TextureType.EmissionColor);
        //texCount += _assimp.GetMaterialTextureCount(material, TextureType.Emissive);
        //texCount += _assimp.GetMaterialTextureCount(material, TextureType.Metalness);
        //texCount += _assimp.GetMaterialTextureCount(material, TextureType.Normals);
        //texCount += _assimp.GetMaterialTextureCount(material, TextureType.Specular);

        if (texCount > 0)
        {
            ProcessMeshVertices(entity, mesh, true);
            ProcessMeshTextures(entity, material, scene, texCount);
            entity.AddComponent(_shaderManager.DefaultTexture);
        }
        else
        {
            ProcessMeshVertices(entity, mesh, false);
            ProcessMeshColor(entity, material);
            entity.AddComponent(_shaderManager.DefaultColor);
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
                var textureName = Path.GetFileName(path);

                string texturePath;
                TextureComponent textureComponent;
                if (textureName.StartsWith('*') && int.TryParse(textureName.AsSpan(1), out var texIdx))
                {
                    var texture = scene->MTextures[texIdx];
                    var extension = Marshal.PtrToStringAnsi((nint)texture->AchFormatHint);
                    texturePath = Path.Combine(_currentDirectory, texture->MFilename) + "." + extension; //TODO: Handle cases where filename is empty

                    if (TextureManager.IsLoaded(texturePath))
                    {
                        textureComponent = TextureManager.GetTexture(texturePath);
                    }
                    else
                    {
                        textureComponent = TextureManager.LoadTexture(texturePath, (byte*)texture->PcData, (int)(texture->MWidth * Math.Max(texture->MHeight, 1) * sizeof(Texel)));
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

    private unsafe uint ProcessLights(Entity? rootEntity, Silk.NET.Assimp.Scene* scene)
    {
        for (int i = 0; i < scene->MNumLights; i++)
        {
            var light = scene->MLights[i];
            var lightNode = GetNodeByName(scene->MRootNode, light->MName.ToString());

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
            entity.AddComponent(new TransformComponent(lightNode->MTransformation));

            rootEntity?.AddChild(entity);
        }

        return scene->MNumLights;
    }

    private unsafe uint ProcessCameras(Entity? rootEntity, Silk.NET.Assimp.Scene* scene)
    {
        for (int i = 0; i < scene->MNumCameras; i++)
        {
            var camera = scene->MCameras[i];
            var cameraNode = GetNodeByName(scene->MRootNode, camera->MName);
            
            var entity = _entityFactory.CreateCamera(camera->MName)
                                       .SetTransformation(cameraNode->MTransformation)
                                       .SetAspectRatio(camera->MAspect)
                                       .SetNearFarClipPlane(camera->MClipPlaneNear, camera->MClipPlaneFar)
                                       .SetFieldOfView(camera->MHorizontalFOV)
                                       .Build();

            rootEntity?.AddChild(entity);
        }

        return scene->MNumCameras;
    }

    private unsafe Node* GetNodeByName(Node* node, string nodeName)
    {
        if (node->MName.ToString() == nodeName)
            return node;

        for (int i = 0; i < node->MNumChildren; i++)
        {
            var childNode = node->MChildren[i];
            if (childNode->MName.ToString() == nodeName)
                return childNode;
        }

        for (int i = 0; i < node->MNumChildren; i++)
        {
            var result = GetNodeByName(node->MChildren[i], nodeName);
            if (result != null)
                return result;
        }

        return null;
    }

    private unsafe Matrix4x4 GetNodeFinalTransform(Node* node)
    {
        var transform = node->MTransformation;

        while (node->MParent != null)
        {
            node = node->MParent;
            if (node->MTransformation != Matrix4x4.Identity)
                transform *= node->MTransformation;
        }

        return transform;
    }

    private unsafe void SceneFileChanged(string filePath)
    {
    }

    public void Dispose()
    {
        _assimp.Dispose();
    }
}

public class MyScript : Script
{
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }
}

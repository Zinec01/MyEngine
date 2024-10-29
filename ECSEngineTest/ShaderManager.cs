using ECSEngineTest.Components;
using ECSEngineTest.Helpers;
using Friflo.Engine.ECS;
using Silk.NET.OpenGL;
using System.Numerics;

namespace ECSEngineTest;

public class ShaderManager
{
    private readonly EntityStore _entityStore;

    private static readonly List<ShaderInfo> _shaders = [];
    private static readonly Dictionary<string, ShaderProgramComponent> _shaderPrograms = [];
    private static readonly Dictionary<string, int> _uniformLocations = [];

    public ShaderProgramComponent DefaultTexture { get; }
    public ShaderProgramComponent DefaultColor { get; }

    public ShaderManager(EntityStore entityStore)
    {
        _entityStore = entityStore;

        DefaultTexture = GetShaderProgram("Default Texture Shader",
                                          @"..\..\..\..\ECSEngineTest\Assets\Shaders\basic_tex.vert",
                                          @"..\..\..\..\ECSEngineTest\Assets\Shaders\basic_tex.frag");

        DefaultColor = GetShaderProgram("Default Color Shader",
                                        @"..\..\..\..\ECSEngineTest\Assets\Shaders\basic_color.vert",
                                        @"..\..\..\..\ECSEngineTest\Assets\Shaders\basic_color.frag");
    }

    public ShaderProgramComponent GetShaderProgram(string name, string vertexPath, string fragmentPath, params ShaderFile[] otherShaders)
    {
        CheckShadersValidity(ref vertexPath, ref fragmentPath, ref otherShaders);

        var programName = GenShaderProgramName(vertexPath, fragmentPath, otherShaders.Select(x => x.FilePath).ToArray());

        if (_shaderPrograms.TryGetValue(programName, out var program))
            return program;

        var vertex = LoadShader(vertexPath, ShaderType.Vertex);
        var fragment = LoadShader(fragmentPath, ShaderType.Fragment);

        var otherShaderInfos = otherShaders.Select(x => LoadShader(x.FilePath, x.Type));

        var component = CreateShaderProgramComponent(name, vertex.Id, fragment.Id, [..otherShaderInfos.Select(x => x.Id)]);

        _shaderPrograms.Add(programName, component);

        return component;
    }

    private static ShaderProgramComponent CreateShaderProgramComponent(string name, uint vertexId, uint fragmentId, uint[] otherShaderIds)
    {
        var id = Window.GL.CreateProgram();

        InitShader(vertexId, id);
        InitShader(fragmentId, id);
        foreach (var shaderId in otherShaderIds)
        {
            InitShader(shaderId, id);
        }

        LinkShaderProgram(id);
        LinkUniformBlocks(id);

        return new ShaderProgramComponent(id, name, vertexId, fragmentId, otherShaderIds);
    }

    private ShaderInfo LoadShader(string filePath, ShaderType type)
    {
        if (_shaders.Any(x => x.FilePath == filePath))
            return _shaders.First(x => x.FilePath == filePath);

        var glType = type switch
        {
            ShaderType.Vertex => Silk.NET.OpenGL.ShaderType.VertexShader,
            ShaderType.Fragment => Silk.NET.OpenGL.ShaderType.FragmentShader,
            ShaderType.Compute => Silk.NET.OpenGL.ShaderType.ComputeShader,
            ShaderType.Geometry => Silk.NET.OpenGL.ShaderType.GeometryShader,
            _ => throw new NotImplementedException()
        };

        var id = Window.GL.CreateShader(glType);

        using (var sr = new StreamReader(filePath))
        {
            Window.GL.ShaderSource(id, sr.ReadToEnd());
        }

        var shaderInfo = new ShaderInfo
        {
            Id = id,
            FilePath = filePath,
            Type = type
        };

        FileChangeWatcher.SubscribeForFileChanges(filePath, (_, _) => ReloadShader(shaderInfo));

        _shaders.Add(shaderInfo);

        return shaderInfo;
    }

    private static void CompileShader(uint shaderId)
    {
        Window.GL.CompileShader(shaderId);

        Window.GL.GetShader(shaderId, ShaderParameterName.CompileStatus, out var status);
        if (status == 0)
        {
            var info = Window.GL.GetShaderInfoLog(shaderId);
            throw new Exception($"Could not compile shader: {info}");
        }
    }

    //public static void CompileAllShaders()
    //{
    //    Parallel.ForEach(_shaders, (shader) => CompileShader(shader.Id));
    //}

    private static void AttachShaderToProgram(uint shaderId, uint programId)
    {
        Window.GL.AttachShader(programId, shaderId);
    }

    private static void DetachShaderFromProgram(uint shaderId, uint programId)
    {
        Window.GL.DetachShader(programId, shaderId);
    }

    private static void DeleteShader(uint shaderId)
    {
        Window.GL.DeleteShader(shaderId);
    }

    private static void InitShader(uint shaderId, uint programId)
    {
        CompileShader(shaderId);
        AttachShaderToProgram(shaderId, programId);
    }

    private static void CheckShadersValidity(ref string vertexPath, ref string fragmentPath, ref ShaderFile[] otherShaders)
    {
        if (!StringHelper.ValidateFilePath(ref vertexPath))
            throw new FileNotFoundException(null, vertexPath);

        if (!StringHelper.ValidateFilePath(ref fragmentPath))
            throw new FileNotFoundException(null, fragmentPath);

        if (otherShaders.Any(x => x.Type == ShaderType.Vertex || x.Type == ShaderType.Fragment))
            throw new InvalidDataException("A shader program cannot contain more than one vertex or fragment shader!");

        for (int i = 0; i < otherShaders.Length; i++)
        {
            if (!StringHelper.ValidateFilePath(ref otherShaders[i].FilePath))
                throw new FileNotFoundException(null, otherShaders[i].FilePath);
        }
    }

    private static void LinkShaderProgram(uint programId)
    {
        Window.GL.LinkProgram(programId);

        Window.GL.GetProgram(programId, ProgramPropertyARB.LinkStatus, out var status);
        if (status == 0)
        {
            var info = Window.GL.GetProgramInfoLog(programId);
            throw new Exception($"Could not link shader program: {info}");
        }
    }

    private static void LinkUniformBlocks(uint programId)
    {
        foreach (var block in ShaderUniforms.UniformBlocks)
        {
            var index = Window.GL.GetUniformBlockIndex(programId, block.Name);
            if (index != uint.MaxValue)
                Window.GL.UniformBlockBinding(programId, index, block.Binding);
        }
    }

    public static void UseShaderProgram(uint programId)
    {
        Window.GL.UseProgram(programId);
    }

    public static void DeleteShaderProgram(uint programId)
    {
        Window.GL.DeleteProgram(programId);
    }

    public static bool SetUniform(uint programId, string varName, int value)
    {
        if (TryGetUniformLocation(programId, varName, out var location))
        {
            Window.GL.Uniform1(location, value);

            return true;
        }

        return false;
    }

    public static bool SetUniform(uint programId, string varName, float value)
    {
        if (TryGetUniformLocation(programId, varName, out var location))
        {
            Window.GL.Uniform1(location, value);

            return true;
        }

        return false;
    }

    public static bool SetUniform(uint programId, string varName, Vector2 value)
    {
        if (TryGetUniformLocation(programId, varName, out var location))
        {
            Window.GL.Uniform2(location, value);

            return true;
        }

        return false;
    }

    public static bool SetUniform(uint programId, string varName, Vector3 value)
    {
        if (TryGetUniformLocation(programId, varName, out var location))
        {
            Window.GL.Uniform3(location, value);

            return true;
        }

        return false;
    }

    public static bool SetUniform(uint programId, string varName, Vector4 value)
    {
        if (TryGetUniformLocation(programId, varName, out var location))
        {
            Window.GL.Uniform4(location, value);

            return true;
        }

        return false;
    }

    public static unsafe bool SetUniform(uint programId, string varName, Matrix4x4 value)
    {
        if (TryGetUniformLocation(programId, varName, out var location))
        {
            Window.GL.UniformMatrix4(location, 1, false, (float*)&value);

            return true;
        }

        return false;
    }

    private static bool TryGetUniformLocation(uint programId, string varName, out int location)
    {
        if (_uniformLocations.TryGetValue(varName, out location))
        {
            return true;
        }
        else
        {
            location = Window.GL.GetUniformLocation(programId, varName);
            _uniformLocations.Add(varName, location);

            return location >= 0;
        }
    }

    private static string GenShaderProgramName(string vertexPath, string fragmentPath, string[]? otherPaths = null)
    {
        var paths = new List<string> { vertexPath, fragmentPath };
        if (otherPaths != null)
        {
            Array.Sort(otherPaths);
            paths.AddRange(otherPaths);
        }

        return string.Join(';', paths);
    }

    private void ReloadShader(ShaderInfo oldShader)
    {
        DeleteShader(oldShader.Id);

        var newShader = LoadShader(oldShader.FilePath, oldShader.Type);
        CompileShader(newShader.Id);

        _shaders[_shaders.IndexOf(oldShader)] = newShader;

        var programKvps = GetAllProgramKvpsContainingShader(oldShader);
        foreach (var programKvp in programKvps)
        {
            var program = programKvp.Value;

            DetachShaderFromProgram(oldShader.Id, program.Id);
            AttachShaderToProgram(newShader.Id, program.Id);
            
            if (oldShader.Type == ShaderType.Vertex)
                program.VertexId = newShader.Id;
            else if (oldShader.Type == ShaderType.Fragment)
                program.FragmentId = newShader.Id;
            else
            {
                for (int j = 0; j < program.OtherIds.Length; j++)
                {
                    if (program.OtherIds[j] == oldShader.Id)
                    {
                        program.OtherIds[j] = newShader.Id;
                        break;
                    }
                }
            }

            LinkShaderProgram(program.Id);

            _shaderPrograms[programKvp.Key] = program;

            _entityStore.Query<ShaderProgramComponent>()
                        .ForEach((components, entities) =>
            {
                var cb = _entityStore.GetCommandBuffer().Synced;
                for (int i = 0; i < entities.Length; i++)
                {
                    if (components[i].Id != program.Id)
                        break;

                    cb.AddComponent(entities.Ids[i], program);
                }
            }).RunParallel();
        }
    }

    private static KeyValuePair<string, ShaderProgramComponent>[] GetAllProgramKvpsContainingShader(ShaderInfo shaderInfo)
    {
        return _shaderPrograms.Where(x => shaderInfo.Type switch
        {
            ShaderType.Vertex   => x.Value.VertexId   == shaderInfo.Id,
            ShaderType.Fragment => x.Value.FragmentId == shaderInfo.Id,
            _                   => x.Value.OtherIds.Contains(shaderInfo.Id)
        }).ToArray();
    }

    private static void DisposeShader(uint shaderId, uint[] programIds)
    {
        foreach (var programId in programIds)
        {
            Window.GL.DetachShader(programId, shaderId);
        }
        Window.GL.DeleteShader(shaderId);
    }

    public static void Dispose()
    {
        foreach (var shader in _shaders)
        {
            var programsContainingShader = GetAllProgramKvpsContainingShader(shader);

            if (programsContainingShader.Length > 0)
            {
                DisposeShader(shader.Id, programsContainingShader.Select(x => x.Value.Id).ToArray());
            }

            FileChangeWatcher.UnsubscribeFromFileChanges(shader.FilePath);
        }
        foreach (var program in _shaderPrograms.Values)
        {
            Window.GL.DeleteProgram(program.Id);
        }

        _shaders.Clear();
        _shaderPrograms.Clear();

        GC.Collect();
    }
}

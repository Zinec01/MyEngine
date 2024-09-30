using ECSEngineTest.Components;
using Silk.NET.OpenGL;
using System.Numerics;

namespace ECSEngineTest;

public static class ShaderManager
{
    private static readonly List<ShaderInfo> _shaders = [];
    private static readonly Dictionary<string, ShaderProgramComponent> _shaderPrograms = [];
    private static readonly Dictionary<string, int> _uniformLocations = [];

    public static ShaderProgramComponent GetShaderProgram(string name, string vertexPath, string fragmentPath, params ShaderFile[] otherShaders)
    {
        CheckShadersValidity(vertexPath, fragmentPath, otherShaders);

        var programName = GenShaderProgramName(vertexPath, fragmentPath, otherShaders.Select(x => x.FilePath).ToArray());

        if (_shaderPrograms.TryGetValue(programName, out var program))
            return program;

        var vertex = LoadShader(vertexPath, ShaderType.Vertex);
        var fragment = LoadShader(fragmentPath, ShaderType.Fragment);

        var otherShaderInfos = otherShaders.Select(x => LoadShader(x.FilePath, x.Type));

        var component = CreateShaderProgram(name, vertex.Id, fragment.Id, [..otherShaderInfos.Select(x => x.Id)]);

        _shaderPrograms.Add(programName, component);

        return component;
    }

    private static ShaderProgramComponent CreateShaderProgram(string name, uint vertexId, uint fragmentId, uint[] otherShaderIds)
    {
        var id = Window.GL.CreateProgram();

        InitShader(vertexId, id);
        InitShader(fragmentId, id);
        foreach (var shaderId in otherShaderIds)
        {
            InitShader(shaderId, id);
        }

        LinkShaderProgram(id);

        return new ShaderProgramComponent(id, name, vertexId, fragmentId, otherShaderIds);
    }

    private static ShaderInfo LoadShader(string filePath, ShaderType type)
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

    public static bool CompileShader(uint shaderId)
    {
        Window.GL.CompileShader(shaderId);

        var info = Window.GL.GetShaderInfoLog(shaderId);
        return string.IsNullOrEmpty(info);
    }

    public static void CompileAllShaders()
    {
        Parallel.ForEach(_shaders, (shader) => CompileShader(shader.Id));
    }

    public static void AttachShaderToProgram(uint shaderId, uint programId)
    {
        Window.GL.AttachShader(programId, shaderId);
    }

    public static void DetachShaderFromProgram(uint shaderId, uint programId)
    {
        Window.GL.DetachShader(programId, shaderId);
    }

    public static void DeleteShader(uint shaderId)
    {
        Window.GL.DeleteShader(shaderId);
    }

    private static void InitShader(uint shaderId, uint programId)
    {
        CompileShader(shaderId);
        AttachShaderToProgram(shaderId, programId);
    }

    private static void CheckShadersValidity(string vertexPath, string fragmentPath, ShaderFile[] otherShaders)
    {
        if (string.IsNullOrEmpty(vertexPath))
            throw new ArgumentNullException(nameof(vertexPath));

        if (!File.Exists(vertexPath))
            throw new FileNotFoundException();

        if (string.IsNullOrEmpty(fragmentPath))
            throw new ArgumentNullException(nameof(fragmentPath));

        if (!File.Exists(fragmentPath))
            throw new FileNotFoundException();

        if (otherShaders.Any(x => x.Type == ShaderType.Vertex || x.Type == ShaderType.Fragment))
            throw new InvalidDataException("A shader program cannot contain more than one vertex or fragment shader!");

        if (otherShaders.Any(x => string.IsNullOrEmpty(x.FilePath)))
            throw new ArgumentNullException(nameof(otherShaders));

        if (otherShaders.Any(x => !File.Exists(x.FilePath)))
            throw new FileNotFoundException();
    }

    public static void LinkShaderProgram(uint programId)
    {
        Window.GL.LinkProgram(programId);

        Window.GL.GetProgram(programId, ProgramPropertyARB.LinkStatus, out var status);
        if (status == 0)
        {
            var info = Window.GL.GetProgramInfoLog(programId);
            Console.WriteLine($"[ERROR] Failed linking shader program with ID = {programId}: {info}");

            Environment.Exit(0);
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

    public static bool SetUniform(uint programId, string varName, Vector3 value)
    {
        if (TryGetUniformLocation(programId, varName, out var location))
        {
            Window.GL.Uniform3(location, value);

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

    private static void ReloadShader(ShaderInfo shaderInfo)
    {
        var programsContainingShader = _shaderPrograms.Where(x => shaderInfo.Type switch
        {
            ShaderType.Vertex => x.Value.VertexId == shaderInfo.Id,
            ShaderType.Fragment => x.Value.FragmentId == shaderInfo.Id,
            _ => x.Value.OtherIds.Contains(shaderInfo.Id)
        });

        if (!programsContainingShader.Any()) return;

        DisposeShader(shaderInfo.Id, programsContainingShader.Select(x => x.Value.Id).ToArray());

        // znovu nacist a zkompilovat shader
        _shaders.Remove(shaderInfo);

        var newShader = LoadShader(shaderInfo.FilePath, shaderInfo.Type);
        CompileShader(newShader.Id);

        // smazat shader programy
        // znovu vytvorit shader programy se vsemi puvodnimi shadery (nahradit pouze id noveho, jinak pouzit existujici id ostatnich shaderu)
        var newPrograms = new List<ShaderProgramComponent>();
        foreach (var programKvp in programsContainingShader)
        {
            var program = programKvp.Value;
            DisposeShaderProgram(program.Id);
            _shaderPrograms.Remove(programKvp.Key);

            var vertexId = newShader.Type == ShaderType.Vertex ? newShader.Id : program.VertexId;
            var fragmentId = newShader.Type == ShaderType.Fragment ? newShader.Id : program.FragmentId;
            var otherIds = newShader.Type is ShaderType.Vertex or ShaderType.Fragment
                            ? program.OtherIds
                            : program.OtherIds.Where(x => x != shaderInfo.Id).Append(newShader.Id).ToArray();

            newPrograms.Add(CreateShaderProgram(program.Name, vertexId, fragmentId, otherIds));
        }

        // nahradit id programu a shaderu v komponentech

        // projit vsechny entity s shader componentem a aktualizovat

        // aktualizovat program componenty v cachi
        // vlozit nove shader info do cache
    }

    public static void DisposeShader(uint shaderId, uint[] programIds)
    {
        foreach (var programId in programIds)
        {
            Window.GL.DetachShader(programId, shaderId);
        }
        Window.GL.DeleteShader(shaderId);
    }

    public static void DisposeShaderProgram(uint programId)
    {
        Window.GL.DeleteProgram(programId);
    }

    public static void Dispose()
    {
        foreach (var shader in _shaders)
        {
            var programsContainingShader = _shaderPrograms.Values.Where(x => shader.Type switch
            {
                ShaderType.Vertex => x.VertexId == shader.Id,
                ShaderType.Fragment => x.FragmentId == shader.Id,
                _ => x.OtherIds.Contains(shader.Id)
            });

            if (programsContainingShader.Any())
            {
                DisposeShader(shader.Id, programsContainingShader.Select(x => x.Id).ToArray());
            }

            FileChangeWatcher.UnsubscribeFromFileChanges(shader.FilePath);
        }
        foreach (var program in _shaderPrograms.Values)
        {
            DisposeShaderProgram(program.Id);
        }

        _shaders.Clear();
        _shaderPrograms.Clear();
    }
}

using Silk.NET.OpenGL;

namespace MyEngine;

internal class Shader : IDisposable
{
    private readonly GL _gl;

    private uint Id { get; }
    public string FilePath { get; }
    public ShaderType Type { get; }

    public bool IsAttached { get; private set; }

    public Shader(GL gl, string filePath, ShaderType type)
    {
        _gl = gl;

        FilePath = filePath;
        Type = type;
        Id = Init(filePath, type);
    }

    private uint Init(string filePath, ShaderType type)
    {
        var id = _gl.CreateShader(type);

        using (var sr = new StreamReader(filePath))
        {
            _gl.ShaderSource(id, sr.ReadToEnd());
        }

        _gl.CompileShader(id);

        var info = _gl.GetShaderInfoLog(id);
        if (!string.IsNullOrEmpty(info))
        {
            Console.WriteLine($"Error with compiling shader at {filePath}:\n{info}");

            Environment.Exit(0);
        }

        return id;
    }

    public void Attach(uint programId)
    {
        _gl.AttachShader(programId, Id);
        IsAttached = true;
    }

    public void Detach(uint programId)
    {
        _gl.DetachShader(programId, Id);
        IsAttached = false;
    }

    public void Dispose(uint programId)
    {
        Detach(programId);
        Dispose();
    }

    public void Dispose()
    {
        _gl.DeleteShader(Id);
    }

    public const string ModelMatrix = "vuModelMat";
    public const string ViewMatrix = "vuViewMat";
    public const string ProjectionMatrix = "vuProjectMat";
    public const string TextureSampler = "fuTex";
}

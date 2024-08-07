using Silk.NET.OpenGL;

namespace MyEngine;

internal class Shader : IDisposable
{
    private uint Id { get; }
    public string FilePath { get; }
    public ShaderType Type { get; }

    public bool IsAttached { get; private set; }

    public Shader(string filePath, ShaderType type)
    {
        FilePath = filePath;
        Type = type;
        Id = Init(filePath, type);
    }

    private static uint Init(string filePath, ShaderType type)
    {
        var id = Game.GL.CreateShader(type);

        using (var sr = new StreamReader(filePath))
        {
            Game.GL.ShaderSource(id, sr.ReadToEnd());
        }

        Game.GL.CompileShader(id);

        var info = Game.GL.GetShaderInfoLog(id);
        if (!string.IsNullOrEmpty(info))
        {
            Console.WriteLine($"Error with compiling shader at {filePath}:\n{info}");

            Environment.Exit(0);
        }

        return id;
    }

    public void Attach(uint programId)
    {
        Game.GL.AttachShader(programId, Id);
        IsAttached = true;
    }

    public void Detach(uint programId)
    {
        Game.GL.DetachShader(programId, Id);
        IsAttached = false;
    }

    public void Dispose(uint programId)
    {
        Detach(programId);
        Dispose();
    }

    public void Dispose()
    {
        Game.GL.DeleteShader(Id);
    }

    public const string ModelMatrix = "v_uModelMat";
    public const string TextureSampler = "f_uTex";
}

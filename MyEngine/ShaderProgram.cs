using Silk.NET.OpenGL;
using System.Numerics;

namespace MyEngine;

internal class ShaderProgram : IDisposable
{
    private readonly GL _gl;

    private uint Id { get; }
    private IList<Shader> Shaders { get; } = [];

    public bool IsLinked { get; private set; } = false;

    public ShaderProgram(GL gl, Shader vertex, Shader fragment)
    {
        _gl = gl;

        Id = gl.CreateProgram();
        Shaders.Add(vertex);
        Shaders.Add(fragment);
    }

    public ShaderProgram(GL gl, string vertexPath, string fragmentPath)
        : this(gl, new Shader(gl, vertexPath, ShaderType.VertexShader), new Shader(gl, fragmentPath, ShaderType.FragmentShader))
    {
    }

    public bool AddShader(Shader shader)
    {
        if (!IsLinked && Shaders.FirstOrDefault(x => x.FilePath == shader.FilePath) == null)
        {
            Shaders.Add(shader);

            return true;
        }

        return false;
    }

    public void AttachShadersAndLinkProgram()
    {
        foreach (var shader in Shaders)
        {
            shader.Attach(Id);
        }

        _gl.LinkProgram(Id);

        _gl.GetProgram(Id, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            var info = _gl.GetProgramInfoLog(Id);
            Console.WriteLine($"Error linking Shader Program: {info}");

            Environment.Exit(0);
        }

        IsLinked = true;
        
        foreach (var shader in Shaders)
        {
            shader.Dispose(Id);
        }
    }

    public void SetUniform(string name, int value)
    {
        var location = GetUniformLocation(name);

        _gl.Uniform1(location, value);
    }

    public void SetUniform(string name, float value)
    {
        var location = GetUniformLocation(name);

        _gl.Uniform1(location, value);
    }

    public void SetUniform(string name, Vector3 value)
    {
        var location = GetUniformLocation(name);

        _gl.Uniform3(location, value);
    }

    public unsafe void SetUniform(string name, Matrix4x4 value)
    {
        var location = GetUniformLocation(name);

        _gl.UniformMatrix4(location, 1, false, (float*)&value);
    }

    private int GetUniformLocation(string name)
    {
        var location = _gl.GetUniformLocation(Id, name);
        if (location < 0)
        {
            Console.WriteLine($"Shader variable {name} not found");

            Environment.Exit(0);
        }

        return location;
    }

    public void Use()
    {
        _gl.UseProgram(Id);
    }

    public void Dispose()
    {
        _gl.DeleteProgram(Id);
    }
}

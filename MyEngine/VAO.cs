using Silk.NET.OpenGL;

namespace MyEngine;

internal class VAO : IDisposable
{
    private readonly GL _gl;

    private uint Id { get; }

    public VAO(GL gl)
    {
        _gl = gl;

        Id = gl.GenVertexArray();
        Bind();
    }

    public void Bind()
    {
        _gl.BindVertexArray(Id);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(Id);
    }
}

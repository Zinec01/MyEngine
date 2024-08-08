using Silk.NET.OpenGL;

namespace MyEngine;

internal class BufferObject<TDataType> : IDisposable where TDataType : unmanaged
{
    private readonly GL _gl;

    private uint Id { get; }

    public unsafe BufferObject(GL gl, ReadOnlySpan<TDataType> data, BufferTargetARB bufferType, BufferUsageARB bufferUsage)
    {
        _gl = gl;

        Id = gl.GenBuffer();
        gl.BindBuffer(bufferType, Id);

        fixed (void* dataPtr = &data[0])
        {
            gl.BufferData(bufferType, (nuint)(data.Length * sizeof(TDataType)), dataPtr, bufferUsage);
        }
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(Id);
    }
}

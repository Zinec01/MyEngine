using Silk.NET.OpenGL;

namespace MyEngine
{
    internal class BufferObject<TDataType> : IDisposable where TDataType : unmanaged
    {
        private uint Id { get; }

        public unsafe BufferObject(ReadOnlySpan<TDataType> vertices, BufferTargetARB bufferType, BufferUsageARB bufferUsage = BufferUsageARB.StaticDraw)
        {
            Id = Program.GL.GenBuffer();
            Program.GL.BindBuffer(bufferType, Id);

            fixed (void* data = &vertices[0])
            {
                Program.GL.BufferData(bufferType, (nuint)(vertices.Length * sizeof(TDataType)), data, bufferUsage);
            }
        }

        public void Dispose()
        {
            Program.GL.DeleteBuffer(Id);
        }
    }
}
